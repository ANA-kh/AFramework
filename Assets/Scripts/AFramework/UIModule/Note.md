# 草稿
uibase
uiscene
ui可以组合,使用事件交流,或者改变model间接交流
mvp + react

page概念,Navigation时保存当前多个ui为一个page,返回时恢复     ui可实现getArguments接口,获取保存的参数.  未实现的ui不会被保存

loader加载器,加载ui prefab. 并记录ui
ui生命周期管理
常用prefab缓存池,例如物品prefab

弹窗队列; 系统通知队列

UIRaycastBlocker

---------------tips
OnHide: 设置canvas的enable,隐藏ui实现0gc.  非ui元素可以添加一个基础UIbehaviour的组件,重写oncanvashirerchychanged接口(对于meshrenderer,设置enabled;particlesystem,用play以及stop进行控制，还有clear接口)
或者设置Canvas的Layer,让相机culling