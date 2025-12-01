Запуск проєкту ShoeShop

Системні вимоги
1) Windows 7/8/10/11
2) .NET Framework 4.7.2+
3) MySQL Server 8.0+
4) Visual Studio 2019/2022
5) Microsoft Office (Excel, Word) для генерації звітів

Шлях до проєкту
WindowsFormsApp2/WindowsFormsApp2.sln

Налаштування бази даних
1. Імпорт БД:
mysql -u root -p < БД.sql
або через MySQL Workbench: Server → Data Import
2. Налаштування підключення:
Відкрити DbConection.cs та змінити параметри:
csharpprivate static string DbConnect = "server=localhost;user=root;password=[ваш_пароль];database=shoestore";


3. Необхідні NuGet пакети:
- `MySql.Data` - підключення до MySQL
- `Microsoft.Office.Interop.Excel` - генерація звітів Excel
- `Microsoft.Office.Interop.Word` - генерація звітів Word

Запуск
1. Переконатися, що MySQL Server запущено
2. Відкрити `WindowsFormsApp2.sln` у Visual Studio
3. Натиснути F5

Облікові дані
Продавці:
Логін: admin / Пароль: admin123
Логін: seller1 / Пароль: seller123
Логін: seller2 / Пароль: seller456

Особливості роботи
Робота з клієнтами:

Введіть номер телефону в кошику для застосування знижки
Поле підсвічується зеленим при знаходженні клієнта

Оформлення продажу:

Автоматичне застосування знижки
Можливість друку чека
Автоматичне оновлення складу

Повернення товарів:

Необхідний номер чека
Обов'язкова причина повернення

Генерація звітів:

Звіт по продажах (Excel/Word)
Звіт по поверненнях (Excel/Word)
Збереження у форматі .xlsx або .docx

Вирішення проблем
Помилка підключення до БД:

Перевірте, чи запущено MySQL Server
Перевірте дані у DbConection.cs
Переконайтеся, що БД shoestore створена

Помилка MySql.Data:

Встановіть пакет через NuGet Package Manager

Помилка Interop (Excel/Word):

Переконайтеся, що встановлено Microsoft Office
Встановіть пакети Microsoft.Office.Interop.Excel та Microsoft.Office.Interop.Word
При необхідності запустіть застосунок від імені адміністратора

Помилка входу:

Перевірте правильність логіна/пароля (чутливі до регістру)

Звіти не генеруються:

Перевірте наявність Microsoft Office на комп'ютері
Закрийте всі відкриті файли Excel/Word перед генерацією звіту
