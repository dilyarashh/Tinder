using Microsoft.EntityFrameworkCore;
using Tinder.DBContext;
using Tinder.DBContext.DTO.User;
using Tinder.DBContext.Models;
using Tinder.Services.IServices;
using Tinder.SupportiveServices.Exceptions;
using Tinder.SupportiveServices.Password;
using Tinder.SupportiveServices.Token;
using Tinder.SupportiveServices.Validations;

namespace Tinder.Services;

public class UserService(AppDbcontext dbcontext, TokenInteractions tokenService, HashPassword hashPassword)
    : IUserService
{
    private bool IsUniqueUserEmail(string email)
    {
        var user = dbcontext.Users.FirstOrDefault(x => x.Email == email);
        if (user == null)
        {
            return true;
        }

        return false;
    }

    public async Task<TokenDTO> Register(RegistrationDTO registrationDto)
    {
        if (!IsUniqueUserEmail(registrationDto.Email))
        {
            throw new BadRequestException("Такой email уже используется");
        }

        if (!NameValidator.IsValidName(registrationDto.FirstName))
        {
            throw new BadRequestException(
                "Неккоректное имя. Введите имя с большой буквы, используйте только буквы. Длина имени не менее двух букв и не более 50");
        }

        if (!NameValidator.IsValidName(registrationDto.LastName))
        {
            throw new BadRequestException(
                "Неккоректная фамилия. Введите фамилию с большой буквы, используйте только буквы. Длина фамилии не менее двух букв и не более 50");
        }

        if (!EmailValidator.ValidateEmail(registrationDto.Email))
        {
            throw new BadRequestException("Неверный формат email");
        }

        if (!BirthdayValidator.ValidateBirthday(registrationDto.BirthDate))
        {
            throw new BadRequestException(
                "Дата рождения должна быть в пределах 01.01.1900 и не позднее нынешнего времени");
        }

        if (!PasswordValidator.IsValidPassword(registrationDto.Password))
        {
            throw new BadRequestException("Пароль должен содержать минимум 8 символов");
        }

        if (!CourseValidator.IsValidCourse(registrationDto.Course, registrationDto.EducationLevel))
        {
            throw new BadRequestException("Если вы бакалавриат - укажите курс от 1 до 4, если магистрант - от 1 до 2");
        }

        if (!TelegramValidator.IsValidTelegram(registrationDto.Telegram))
        {
            throw new BadRequestException("Укажите корректный телеграм");
        }

        if (registrationDto.PhotoUrl != null && !UrlValidator.IsValidUrl(registrationDto.PhotoUrl))
        {
            throw new BadRequestException("Укажите корректную ссылку на фотографию");
        }

        string hashedPassword = hashPassword.HashingPassword(registrationDto.Password);

        int CalculateAge(DateTime birthDate)
        {
            var today = DateTime.Today;
            var age = today.Year - birthDate.Year;
            if (birthDate.Date > today.AddYears(-age)) age--;
            return age;
        }

        User user = new User()
        {
            Id = Guid.NewGuid(),
            FirstName = registrationDto.FirstName,
            LastName = registrationDto.LastName,
            BirthDate = registrationDto.BirthDate,
            Age = CalculateAge(registrationDto.BirthDate),
            EducationLevel = registrationDto.EducationLevel,
            Course = registrationDto.Course,
            Password = hashedPassword,
            Email = registrationDto.Email,
            Telegram = registrationDto.Telegram,
            PhotoUrl = registrationDto.PhotoUrl,
            About = registrationDto.About,
            Gender = registrationDto.Gender
        };

        await dbcontext.Users.AddAsync(user);
        await dbcontext.SaveChangesAsync();

        var token = tokenService.GenerateToken(user);
        return new TokenDTO
        {
            Token = token
        };
    }

    public async Task<TokenDTO> Login(LoginDTO loginDto)
    {
        if (!EmailValidator.ValidateEmail(loginDto.Email))
        {
            throw new BadRequestException("Неверный формат email. Для авторизации введите ваш email.");
        }

        var user = await dbcontext.Users.FirstOrDefaultAsync(d => d.Email == loginDto.Email);
        if (user == null)
        {
            throw new BadRequestException("Неправильный email");
        }
        else if (!HashPassword.VerifyPassword(loginDto.Password, user.Password))
        {
            throw new BadRequestException("Неправильный пароль");
        }

        var token = tokenService.GenerateToken(user);
        return new TokenDTO
        {
            Token = token
        };
    }

    public async Task<UserDTO> GetProfile(string? token)
    {
        var userId = tokenService.GetIdFromToken(token);
        var user = await dbcontext.Users.FirstOrDefaultAsync(d => d.Id == Guid.Parse(userId));
        if (user != null)
        {
            return new UserDTO
            {
                Id = Guid.NewGuid(),
                FirstName = user.FirstName,
                LastName = user.LastName,
                BirthDate = user.BirthDate,
                EducationLevel = user.EducationLevel,
                Course = user.Course,
                Email = user.Email,
                Telegram = user.Telegram,
                PhotoUrl = user.PhotoUrl,
                About = user.About,
                Gender = user.Gender,
                Age = user.Age,
            };
        }
        else
        {
            throw new UnauthorizedException("Пользователь не авторизован");
        }
    }

    public async Task EditProfile(string? token, EditDTO editDto)
    {
        var userId = tokenService.GetIdFromToken(token);
        var user = await dbcontext.Users.FirstOrDefaultAsync(d => d.Id == Guid.Parse(userId));

        if (user == null)
        {
            throw new UnauthorizedException("Пользователь не авторизован");
        }

        // Имя
        if (editDto.FirstName != null)
        {
            if (!NameValidator.IsValidName(editDto.FirstName))
            {
                throw new BadRequestException(
                    "Некорректное имя. Введите имя с большой буквы, используйте только буквы. Длина имени не менее двух букв");
            }

            user.FirstName = editDto.FirstName;
        }

        // Фамилия
        if (editDto.LastName != null)
        {
            if (!NameValidator.IsValidName(editDto.LastName))
            {
                throw new BadRequestException(
                    "Некорректная фамилия. Введите фамилию с большой буквы, используйте только буквы. Длина фамилии не менее двух букв");
            }

            user.LastName = editDto.LastName;
        }

        // Email
        if (editDto.Email != null)
        {
            if (!IsUniqueUserEmail(editDto.Email))
            {
                throw new BadRequestException("Такой email уже используется");
            }

            if (!EmailValidator.ValidateEmail(editDto.Email))
            {
                throw new BadRequestException("Неверный формат email");
            }

            user.Email = editDto.Email;
        }

        // Дата рождения и возраст
        if (editDto.BirthDate != null)
        {
            if (!BirthdayValidator.ValidateBirthday(editDto.BirthDate.Value))
            {
                throw new BadRequestException(
                    "Дата рождения должна быть в пределах 01.01.1900 и не позднее нынешнего времени");
            }

            user.BirthDate = editDto.BirthDate.Value;

            int CalculateAge(DateTime birthDate)
            {
                var today = DateTime.Today;
                var age = today.Year - birthDate.Year;
                if (birthDate.Date > today.AddYears(-age)) age--;
                return age;
            }

            user.Age = CalculateAge(editDto.BirthDate.Value);
        }

        // Уровень образования
        if (editDto.EducationLevel != null)
        {
            user.EducationLevel = editDto.EducationLevel.Value;
        }

        // Курс (с учётом educationLevel)
        if (editDto.Course != null)
        {
            var educationLevel = editDto.EducationLevel ?? user.EducationLevel;

            if (!CourseValidator.IsValidCourse(editDto.Course.Value, educationLevel))
            {
                throw new BadRequestException(
                    "Если вы бакалавриат - укажите курс от 1 до 4, если магистрант - от 1 до 2");
            }

            user.Course = editDto.Course.Value;
        }

        // Телеграм
        if (editDto.Telegram != null)
        {
            if (!TelegramValidator.IsValidTelegram(editDto.Telegram))
            {
                throw new BadRequestException("Укажите корректный телеграм");
            }

            user.Telegram = editDto.Telegram;
        }

        // Фото
        if (editDto.PhotoUrl != null)
        {
            if (!UrlValidator.IsValidUrl(editDto.PhotoUrl))
            {
                throw new BadRequestException("Укажите корректную ссылку на фотографию");
            }

            user.PhotoUrl = editDto.PhotoUrl;
        }

        // Описание
        if (editDto.About != null)
        {
            user.About = editDto.About;
        }

        // Гендер
        if (editDto.Gender != null)
        {
            user.Gender = editDto.Gender.Value;
        }

        await dbcontext.SaveChangesAsync();
    }


    public async Task Logout(string? token)
    {
        string id = tokenService.GetIdFromToken(token);

        if (Guid.TryParse(id, out Guid userId) && userId != Guid.Empty)
        {
            await dbcontext.BlackTokens.AddAsync(new BlackToken { Blacktoken = token, CreatedAt = DateTime.UtcNow });
            await dbcontext.SaveChangesAsync();
        }
        else
        {
            throw new UnauthorizedException("Не удалось осуществить выход");
        }
    }
}