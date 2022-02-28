# [EN] OAuth2.Client
Small library for authentication of users on the third-party services supporting OAuth2 protocol: Google, Yandex, etc - nothing more, only authorization and obtaining information about the user. Deep rework of the project https://github.com/titarenko/OAuth2.

The main difference is that the user data model is simplified as much as possible, the second important point is that the implemented clients can be configured once and registered in the IServiceCollection as singleton objects, the methods is thread-safe.

Clients can create instance of the inherited user's data model - this is sometimes necessary when there are additional fields in the database and you can immediately save the created model without having to transfer data between objects.

The infrastructure is also prepared to verify work with services and testing data.

## Current Version and Status
First release, version 0.1, I use in your projects. At the moment, work has been implemented:
* GitHub
* Google
* MicrosoftLive
* VK
* Yandex

A test site has been made to verify work with services. XUnit tests are made as close as possible to real work, but without real access to services

## Usage
1. Installing a package using [NuGet](http://www.nuget.org/packages/Dm.OAuth2.Client/)
```shell
dotnet add package Dm.OAuth2.Client
```
2. Configure client
```c#
// Somewhere in the service configuration
Services.AddSingleton(new OAuth2.Client.For.Google(new Options
{
    ClientID     = "...",
    ClientSecret = "...",
    Scope        = "...",
    RedirectURI  = "..."
}));
```
3. Create login URL
```c#
var url = HttpContext.RequestServices.GetServices<OAuth2.Client.For.Google>().GetLoginURIAsync(state);
// Output url in the template
```

4. Getting information from service after authorization by user
```c#
public async Task<IActionResult> Callback(string code, string? state)
{
    try
    {
        var client = HttpContext.RequestServices.GetServices<OAuth2.Client.For.Google>();
        var user   = await client.GetUserInfoAsync(Request.Query);

        // Work with user data

        return RedirectOnSuccess(user);
    } catch(Exception ex)
    {
        return RedirectToError(ex);
    }
}
```
A more complete example of using clients can be found in the [OAuth2.Client.TestWeb](https://github.com/ETCDema/OAuth2.Client/tree/master/OAuth2.Client.TestWeb) project.

## Create a new client for the service
Steps to create a new client for the service:
1. Implement a class inheritor `SomeService<TUserInfo> : OAuth2Based<TUserInfo>` and `SomeService : SomeService<UserInfo>`
1. Add it necessary to the configuration of the test site `OAuth2.Client.TestWeb` and debug work with the service
1. Based on the data exchange log of the test site `OAuth2.Client.TestWeb` and the service, create a test in the project `OAuth2.Client.XUnitTest`

==Need help translating documentation and comments into English.==

* * *

# [RU] OAuth2.Client
Небольшая библиотека для идентификации пользователей на сторонних сервисах, поддерживающих OAuth2 протокол: Google, Yandex и т.д. - ничего лишнего, только авторизация и получение информации о пользователе. Глубокая переработка проекта https://github.com/titarenko/OAuth2.

Основное отличие - максимально упрощена модель данных пользователя, второй важный момент - реализованные клиенты могут быть сконфингурированы один раз и зарегистрированы в IServiceCollection как singleton объекты, работа методов потокобезопасна.

Клиенты могут создавать наследников модели данных пользователя - это иногда необходимо, когда в базе данных есть дополнительные поля и можно сразу сохранять созданную модель без необходимости переносить данные между моделями.

Так же подготовлена инфраструктура для проверки работы с сервисами и тестирования обмена данными.

## Текущая версия и статус
Первый выпуск, версия 0.1, использую в своих проектах. На данный момент реализована работа с сервисами:
* GitHub
* Google
* MicrosoftLive
* VK
* Yandex

Сделан тестовый сайт для проверки работы с сервисами. Сделаны тесты, максимально приближенные к реальной работе, но без реального обращения к сервисам.

## Использование
1. Установка пакета с использованием [NuGet](http://www.nuget.org/packages/Dm.OAuth2.Client/)
```shell
dotnet add package Dm.OAuth2.Client
```
2. Настройка клиента
```c#
// Где-то в конфигурации сервисов
Services.AddSingleton(new OAuth2.Client.For.Google(new Options
{
    ClientID     = "...",
    ClientSecret = "...",
    Scope        = "...",
    RedirectURI  = "..."
}));
```
3. Получение ссылки для входа
```c#
var url = HttpContext.RequestServices.GetServices<OAuth2.Client.For.Google>().GetLoginURIAsync(state);
// Вывод url в шаблоне
```

4. Получение информации от сервиса после авторизации пользователем
```c#
public async Task<IActionResult> Callback(string code, string? state)
{
    try
    {
        var client = HttpContext.RequestServices.GetServices<OAuth2.Client.For.Google>();
        var user   = await client.GetUserInfoAsync(Request.Query);

        // Работа с полученными данными

        return RedirectOnSuccess(user);
    } catch(Exception ex)
    {
        return RedirectToError(ex);
    }
}
```
Более полный пример использования клиентов можно найти в проекте [OAuth2.Client.TestWeb](https://github.com/ETCDema/OAuth2.Client/tree/master/OAuth2.Client.TestWeb).

## Создание клиента для сервиса
Для создания клиента для сервиса нужно:
1. Реализовать наследника `SomeService<TUserInfo> : OAuth2Based<TUserInfo>` и `SomeService : SomeService<UserInfo>`
1. Добавить необходимое в конфигурацию тестового сайта `OAuth2.Client.TestWeb` и отладить работу с сервисом
1. На основе лога обмена данными тестового сайта `OAuth2.Client.TestWeb` и сервиса создать тест в проекте `OAuth2.Client.XUnitTest`

==Требуется помощь в переводе документации и комментариев на английский язык.==
