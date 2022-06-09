# Cirilla

与huatuo热更进行适配的unity3d开发框架（未集成huatuo热更框架，需要的可自行部署:https://github.com/focus-creative-games/huatuo）。

基础模块以依赖注入与控制反转的方式进行加载和使用(IocContainer)。

因为适配huatuo的原因，开发目录是以单独的程序集进行的(提供了相应的生成器和Editor菜单)，入口以流程状态机的形式提供。

Ciriila提供了一套以文件夹分类、0依赖的资源管理机制，并提供了相应的打包工具和资源读取模块。

提供了基础的MVC开发模块，MVC代码通过注入和主动加载两种方式使用。其中为view层提供了UI和非UI的收集和代码生成功能。

目前提供了基础的模块包括：

资源模块 ->IResModule

网络模块 ->INetModule

订阅器模块 ->IObserverModule

MVC模块 ->IMVCModule

CSV模块 ->ICSVModule

声音模块 ->IAudioModule

后续会根据业务情况总结迭代更多可复用的功能

