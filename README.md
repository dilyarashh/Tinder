КАК ПОДКЛЮЧИТЬСЯ К БД И ЗАПУСТИТЬ:
1. Идем в файлик appsettings.json и в "DefaultConnection" прописываем ваш порт, имя пользователя и пароль. Базу данных менять не надо и вручную ее создавать тоже не надо.
2. В терминале пишем dotnet ef migrations add InitialCreate
3. В терминале пишем dotnet ef database update
4. Запускаем проект
5. Открываем http://localhost:5050/swagger
