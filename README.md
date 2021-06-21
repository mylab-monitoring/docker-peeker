# MyLab.HttpMetrics
[![NuGet Version and Downloads count](https://buildstats.info/nuget/MyLab.HttpMetrics)](https://www.nuget.org/packages/MyLab.HttpMetrics)

```
Поддерживаемые платформы: .NET Core 3.1+
```
Ознакомьтесь с последними изменениями в [журнале изменений](/changelog.md).

## Обзор

Использует стандартную библиотеку [Prometheus для .NET Core](https://github.com/prometheus-net/prometheus-net) для предоставления основных метрик, связанных с выполнением HTTP запросов.

Для добавления метрик при конфигурировании приложения необходимо с помощью методов расширения:

1. `AddUrlBasedHttpMetrics` - добавить необходимые зависимости 
2. `UseUrlBasedHttpMetrics` - добавить `middleware` для перехвата и анализа запросов 

```C#
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddUrlBasedHttpMetrics();		// <--- 1
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();

        app.UseUrlBasedHttpMetrics();			// <--- 2

        app.UseEndpoints(endpoints =>
		{
        	endpoints.MapControllers();
            endpoints.MapMetrics();
        });
    }
}
```

Пример вывода собираемых метрик (подрезано):

```
# HELP ml_http_request_count_total The total number of requests
# TYPE ml_http_request_count_total counter
ml_http_request_count_total{method="GET",path="/get/xxx",status_code="200"} 1
# HELP ml_http_request_content_size_bytes The request content size in bytes.
# TYPE ml_http_request_content_size_bytes histogram
ml_http_request_content_size_bytes_sum{method="GET",path="/get/xxx",status_code="200"} 0
ml_http_request_content_size_bytes_count{method="GET",path="/get/xxx",status_code="200"} 1
ml_http_request_content_size_bytes_bucket{method="GET",path="/get/xxx",status_code="200",le="1024"} 1
...
ml_http_request_content_size_bytes_bucket{method="GET",path="/get/xxx",status_code="200",le="+Inf"} 1
# HELP ml_http_request_size_bytes_total The total size of request content
# TYPE ml_http_request_size_bytes_total counter
# HELP ml_http_request_duration_seconds The duration in seconds between the response to a request.
# TYPE ml_http_request_duration_seconds histogram
ml_http_request_duration_seconds_sum{method="GET",path="/get/xxx",status_code="200"} 0.0303877
ml_http_request_duration_seconds_count{method="GET",path="/get/xxx",status_code="200"} 1
ml_http_request_duration_seconds_bucket{method="GET",path="/get/xxx",status_code="200",le="0.01"} 0
...
ml_http_request_duration_seconds_bucket{method="GET",path="/get/xxx",status_code="200",le="30"} 1
ml_http_request_duration_seconds_bucket{method="GET",path="/get/xxx",status_code="200",le="+Inf"} 1
# HELP process_working_set_bytes Process working set
# TYPE process_working_set_bytes gauge
process_working_set_bytes 111038464
# HELP ml_http_response_content_size_bytes The response content size in bytes.
# TYPE ml_http_response_content_size_bytes histogram
ml_http_response_content_size_bytes_sum{method="GET",path="/get/xxx",status_code="200"} 0
ml_http_response_content_size_bytes_count{method="GET",path="/get/xxx",status_code="200"} 1
ml_http_response_content_size_bytes_bucket{method="GET",path="/get/xxx",status_code="200",le="1024"} 1
...
ml_http_response_content_size_bytes_bucket{method="GET",path="/get/xxx",status_code="200",le="+Inf"} 1
```

## Метрики

### `ml_http_request_count_total`

Счётчик. Считает количество поступивших запросов.

### `ml_http_unhandled_exception_total`

Счётчик. Считает количество необработанных исключений.

### `ml_http_request_content_size_bytes`

Гистограмма. Ведёт учёт объёма запросов в байтах. Учёт основан на значениях из заголовка `Content-Length`.

Счётчики гистограммы:

* 1kb
* 2kb                
* 5kb       
* 10kb       
* 20kb       
* 50kb                
* 1Mb

### `ml_http_response_content_size_bytes` 

Гистограмма. Ведёт учёт объёма ответов в байтах. Учёт основан на количестве данных, записанных в выходной поток тела сообщения ответа.

Счётчики гистограммы:

* 1kb
* 2kb                
* 5kb       
* 10kb       
* 20kb       
* 50kb                
* 1Mb

### `ml_http_request_duration_seconds`

Гистограмма. Ведёт учёт длительности обработки запросов в секундах. 

Счётчики гистограммы:

- 10ms
- 20ms
- 50ms
- 100ms
- 200ms
- 500ms
- 1s
- 2s
- 3s
- 10s
- 20s
- 30s

### Метки 

* `method` - HTTP-метод в верхнем регистре
* `path` - нормализованный путь из `URL` запроса
* `status_code` - числовой статус-код ответа

### Нормализация пути URL

Основной целью нормализации является приведение меток метрик, содержащих пути URL запросов, содержащих идентификаторы, к единому виду.

Выполняются следующие манипуляции:

* адрес приводится к нижнему регистру
* проверяется каждый элемент пути и заменяется на `xxx`, если:
  * цифр больше остальных символов
  * это `GUID`
  * содержит последовательность из цифр длиной 3 и более

### Нормализация перечислений 

Для нормализации значений перечислений в качестве значений меток можно применять `EnumConverter.ToLabel`. 

Примеры конвертирования:

* `TestEnum.Foo` => "foo"
* `TestEnum.Foo1` => "foo_1"
* `TestEnum.Foo2Bar` => "foo_2_bar"
* `TestEnum.FooBar` => "foo_bar"
* `TestEnum.Foo | TestEnum.FooBar` => "foo_foo_bar"
* `(TestEnum)50` => "50"