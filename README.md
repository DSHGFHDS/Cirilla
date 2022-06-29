# README

# Cirilla

轻量、友好、易维护的Unity3d框架。

QQ群 765086420

![Cirilla.png](README%20f78846bd73a4464c83215defbba92065/Cirilla.png)

# 快速开始

## 安装

1. 基于目前(2022.6.15)huatuo框架的更新情况，推荐使用unity3d 2020.3.33版本。
2. 为当前版本安装il2cpp：UnityHub→安装→对应版本设置（齿轮图标）→添加模块(不存在这个选项的因为你没有从Unity3dHub安装这个版本)→il2cpp。
3. 安装huatuo热更框架:Unity3d打开Cirilla→头顶工具栏→HuaTuo→manager…

![pic3.png](README%20f78846bd73a4464c83215defbba92065/pic3.png)

## 配置

1. **项目配置表**
    
    Asset下建立文件夹(更改为你的项目名称)，并在配置表中选定为需要进行开发的项目文件即可。 
    
2. **流程配置表**
    
    通过编辑器创建流程文件（或手动写继承流程Asbtract）并添加到流程配置表中，项目启动会自动进入入口流程。
    
    ![pic1.png](README%20f78846bd73a4464c83215defbba92065/pic1.png)
    
    ![pic2.png](README%20f78846bd73a4464c83215defbba92065/pic2.png)
    

## 资源打包、build和热更

- 打包途径
    
    1.头顶工具栏→Cirilla→工具→资源管理器
    
    ![pic4.png](README%20f78846bd73a4464c83215defbba92065/pic4.png)
    
    2.直接build项目
    
- build注意事项（[https://focus-creative-games.github.io/huatuo/start_up/#注意事项](https://focus-creative-games.github.io/huatuo/start_up/#%E6%B3%A8%E6%84%8F%E4%BA%8B%E9%A1%B9)）
    
    ![pic5.png](README%20f78846bd73a4464c83215defbba92065/pic5.png)
    
- 热更
    1. 修改项目代码或资源以后进行打包或build，Asset下生成StreamingAssets文件夹，里面就是打好的资源包，非移动端下替换到build好的游戏项目Data里的StreamingAssets即刻，具体包的对应信息会在打包后显示在控制台，可对照进行替换需要热更的包。移动端下(安卓、IOS)因为不支持StreamingAssets的修改，热更需要自行将热更文件下载到persistenData里。
    2. 目前暂未提供与服务器对比并更新包的功能，有需求的可自行在项目业务中开发，具体操作域在HotBuffer中。
        
        ![T1.png](README%20f78846bd73a4464c83215defbba92065/T1.png)
        

# 介绍

## **MVC设计规范**

Cirilla认为并非只有UI业务才适用MVC架构，MVC是一种规范，不应该受限于某种具体的业务行为，应该把一切处理unity3d场景内相关的事务视为View层(如物体渲染、创建和销毁物体、控制物体变换等等)，再根据业务划分不同粒度的Model和Controller以进行管理。

Cirilla采用IOS主动MVC的设计，Controller持有View与Model，V与M之间绝对隔离，V和M通过事件驱动主动通知C进行工作。

![D3.png](README%20f78846bd73a4464c83215defbba92065/D3.png)

### UIView/ObjectView

1. 右键Hierarchy→CirillaView可创建两种View。其中UIView带有Canvas相关组件，为UI而设计的，其余并无差别。创建后，可在UIView中设计常用的菜单界面、ObjectView则可以用来设计常用的场景。完成后，拖制成预制体。
    
    ![D1.png](README%20f78846bd73a4464c83215defbba92065/D1.png)
    
    ![D2.png](README%20f78846bd73a4464c83215defbba92065/D2.png)
    
2. 拖制成预制体后，在其中可以看到拖拽框和代码生成按钮。将View层下的、需要被生成到View层代码中以使用的子物体拖拽到框中，最后点击生成代码，就会生成相应的View层代码了。
    
    ![D4.png](README%20f78846bd73a4464c83215defbba92065/D4.png)
    
    ![D5.png](README%20f78846bd73a4464c83215defbba92065/D5.png)
    

### Controller/Model

1. Controller和Model的代码需要自己手动创建，并且继承相应的IController和IModel。
2. 在流程中通过[Controller]标签依赖注入Controller实例（也可以通过mvc模块主动加载）。
    
    ![D6.png](README%20f78846bd73a4464c83215defbba92065/D6.png)
    
3. 因为View和Model相对隔离，并且只能被Controller所持有，所有V和M只允许在Controller中被注入实例。当然，View和Model可以被多个Controller同时持有，它们的实例也仅有且只有一个。
    
    ![D7.png](README%20f78846bd73a4464c83215defbba92065/D7.png)
    
4. Controller持有的View和Model，可以通过MV提供的公共方法进行操作(当然也可以通过事件分发机制进行完全解耦，随你)。而View和Model则只能通过事件分发机制对Controller进行主动通讯。

## 资源管理

Ciriila提供了一套以文件夹分类、0依赖为理念的资源管理策略，并提供了相应的打包工具和资源模块。build或在编辑器打包工具中进行打包，会自动搜索RawResources，以文件夹的形式分包进行打包，所依赖的资源一并打入相应的包中，所以包与包之间不存在依赖关系（为了减少文件冗余，应尽量将相同依赖的不同文件放在一个文件夹下）。打包过程中会将生成的项目代码dll也一并打包，具体信息在打包后会在控制台一一输出。

- 不同命名尾缀的文件夹：
    - _public
        
        项目启动后立刻加载 并不在释放
        
    - _custom
        
         手动加载和手动释放
        
    - _base
        
        用来存在临时素材，打包的时候会忽略掉该文件夹以及其子文件夹
        
    - 无尾缀
        
        根据使用情况动态加载和释放 
        
    

## 功能模块

基于控制反转和依赖注入的理念，基础模块通过Ioc容器进行绑定、加载和使用。

后续会根据业务情况总结迭代更多可复用的功能。

- **资源模块 -> IResModule**
    1. 基于打包策略设计，资源统一管理。
    2. 使用简单，提供同步、异步加载、释放功能。
- **网络模块 -> INetModule**
    1. 可继承的网络实现:WebSocket、TCP，提供连接、断连、接受、发送功能。
    2. 基于lambda的Http请求功能。
- **订阅器模块 -> IObserverModule**
    1. 提供以枚举为ID、传递不定参的事件订阅、脱订、触发。
- **对象池管理模块 -> GoPoolModule**
    1. 提供预制体注册、实例申请、实例回收等基本功能。
    2. 内部通过表校验和队列分配进行统一管理。
    3. 强烈要求使用此模块管理一切游戏对象，禁止实例化和销毁。
- **声音模块 -> IAudioModule**
    1. 提供2D声音、定点声音和追随声音功能。
    2. 基于lambda的结束回调和心跳回调。
- **CSV模块 -> ICSVModule（设计的不太好，后续会取缔掉）**
    1. 读取csv文件到属性表
    2. 属性表写入csv文件
- **MVC模块 -> IMVCModule**
    1. 以ioc容器为基础，所有MVC代码均注册进ioc容器中，通过只允许依赖注入的方式限制MVC规范。
    2. 提供View层框架的相关编辑器工具和代码生成器。