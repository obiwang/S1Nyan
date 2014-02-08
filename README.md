# S1 Nyan

S1 Nyan is a Windows Phone App for the online forum [Stage1st](http://bbs.saraba1st.com/2b) in China. You can find this app on Windows Phone Marketplace (search for `S1` or click [here](http://www.windowsphone.com/s?appid=61790166-792c-493b-bcc2-a2f1506292f5)).

## Major change

1.2

* 添加收藏功能
	
	Add 'Add thread to favorite'

* 添加“我的收藏，我的主题，我回复的主题”

	Add 'My favorite', 'My thread', 'My replied thread'

1.1

* 添加登陆和回复功能

    Add login and reply

* 添加动态服务器列表支持

    Add dynamic server support

* 图片可长按在浏览器中打开

    Images now can be opened in the Internet Explorer from context menu


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

    Custom Theme

## Third party libraries in use

Most of these libraries can be found in nuget

* [Coding4Fun Toolkit](http://coding4fun.codeplex.com/) - GZipWebClient & controls
* [Microsoft.Bcl](http://nuget.org/packages/Microsoft.Bcl/) - async support
* [PhoneThemeManager](http://github.com/jeffwilcox/wp-thememanager/) - switch theme
* [ImageTools](http://imagetools.codeplex.com/) - decode & display gif images
* [MemoryDiagnosticsHelper](http://nuget.org/packages/MemoryDiagnosticsHelper/) - debug helper
* [Caliburn.Micro](https://caliburnmicro.codeplex.com/) - MVVM framework
* [Moq](https://code.google.com/p/moq/) - unit test mocks
* [Phone toolkit](http://phone.codeplex.com/) - many useful controls

## License

The MIT License (MIT)
