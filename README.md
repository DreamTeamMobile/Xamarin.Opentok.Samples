# [Vonage Video API (previously OpenTok)](https://www.vonage.com/communications-apis/video/) Xamarin iOS/Android sample project

Sample showing how to integrate Vonage Video API (Opentok) SDK into Xamarin iOS/Android apps

The Vonage Video API SDK allows you to build live interactive video, voice and messaging into your web and mobile apps

## Xamarin.iOS and Xamarin.Android OpenTok SDK packages

Xamarin.OpenTok.Android - includes all capabilities [![NuGet Badge](https://buildstats.info/nuget/Xamarin.OpenTok.Android)](https://www.nuget.org/packages/Xamarin.OpenTok.Android/)

Xamarin.OpenTok.iOS - includes all capabilities [![NuGet Badge](https://buildstats.info/nuget/Xamarin.OpenTok.iOS)](https://www.nuget.org/packages/Xamarin.OpenTok.iOS/)


### Additional logging capabilities for Xamarin iOS SDK

To enable additional logging on iOS, please call this method before any Vonage SDK activity:

```
            OpenTok.OpenTokExtraLogging.EnableOpenTokLoggingToConsole();
```

The output will be visible in Visual Studio _Application Output_ window:

```

*******************************************************
NOTICE: OPENTOK CONSOLE LOGGER HAS NOT BEEN SET.
PLEASE USE otk_console_set_logger(otk_console_logger)
TO SET YOUR LOGGER.
*******************************************************
2020-08-28 05:07:40,904 UTC [DEBUG] otk_peer_connection.cpp:1174 - Thread 0xc07d480f01000000 - otk_enable_webrtc_trace[otk_enable_webrtc_trace_levels level=0]
2020-08-28 01:07:40.906735-0400 DT.Samples.Opentok.OneToOne.iOS[99671:3738797] 
2020-08-28 01:07:40.906816-0400 DT.Samples.Opentok.OneToOne.iOS[99671:3738797] ------------------------------------------------
2020-08-28 01:07:40.906894-0400 DT.Samples.Opentok.OneToOne.iOS[99671:3738797] OpenTok iOS Library, Rev.2
2020-08-28 01:07:40.906950-0400 DT.Samples.Opentok.OneToOne.iOS[99671:3738797] This build was born on Jul 16 2020 at 01:19:25
2020-08-28 01:07:40.912500-0400 DT.Samples.Opentok.OneToOne.iOS[99671:3738797] Version: 2.17.1.9206-ios
2020-08-28 01:07:40.912598-0400 DT.Samples.Opentok.OneToOne.iOS[99671:3738797] libOpenTokObjC:9beaf26eb6282257d2a4e64e8ea859953136f272
2020-08-28 01:07:40.912864-0400 DT.Samples.Opentok.OneToOne.iOS[99671:3738797] Licensed under the Apache License, Version 2.0
2020-08-28 01:07:40.913036-0400 DT.Samples.Opentok.OneToOne.iOS[99671:3738797] ------------------------------------------------
2020-08-28 01:07:40.915607-0400 DT.Samples.Opentok.OneToOne.iOS[99671:3739045] [DEBUG] otk_ssl_util.c:245 - otk_ssl_static_init_default[]
2020-08-28 01:07:40.917427-0400 DT.Samples.Opentok.OneToOne.iOS[99671:3739045] [DEBUG] otk_ssl_util.c:35 - otk_ssl_enable_verify_peer_impl[]
...

```

## License

The MIT License (MIT).
