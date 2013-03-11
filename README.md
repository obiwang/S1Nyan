# S1 Nyan

S1 Nyan is a Windows Phone 7 application for the online forum [Stage1st](http://bbs.saraba1st.com/2b) in China. You can find this app on Windows Phone Marketplace (search for `S1` or click [here](http://www.windowsphone.com/s?appid=61790166-792c-493b-bcc2-a2f1506292f5)).

## Major change

1.0 init ver.

* 针对手机系统专门优化，省流量且更流畅。

    using GzipWebClient to save bytes

* 基本浏览功能

    browse through the forum

* 图片手动与自动显示

    Image auto/manual display

* 支持屏幕旋转锁定

    Orientation lock

* S1主题与系统主题随意切换

    Theme change
    
## What's next

1.1

* 添加登陆和回复

    add login and reply


## Third party libraries in use

Most of these libraries are import from nuget, which will be downloaded during build phrase for the first time.

* [Coding4Fun Toolkit](http://coding4fun.codeplex.com/) - GZipWebClient & controls
* [Microsoft.Bcl](http://nuget.org/packages/Microsoft.Bcl/) - for async tasks
* [PhoneThemeManager](http://github.com/jeffwilcox/wp-thememanager/) - for theme changing
* [ImageTools](http://imagetools.codeplex.com/) - for decode & display gif images
* [MemoryDiagnosticsHelper](http://nuget.org/packages/MemoryDiagnosticsHelper/) - for debug
* [Mvvm light portable](http://mvvmlight.codeplex.com/SourceControl/network/forks/onovotny/MvvmLightPortable) - MVVM framework
* [Rhino Mocks](http://hibernatingrhinos.com/oss/rhino-mocks) - unit test mocks
* [Phone toolkit](http://phone.codeplex.com/) - many controls