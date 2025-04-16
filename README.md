# DancingLineTemplate-Renewed
A unity project aiming to make a template as the game DancingLine.
## 模板下载
该项目默认分支为master,此分支提供模板的基础版本，项目文件较小。如果你需要一个示例关卡用于学习模板中的部分功能，请下载main分支中的项目<br>

4/16由于部分功能重写,main分支中的关卡实例需要修复，请先下载master分支中的项目
## 关于作者
[Bilibili个人页](https://space.bilibili.com/291841883?spm_id_from=333.1007.0.0)

## 关于项目
  Project created in 2025/3
## 支持
  [OdinInspector](https://odininspector.com/)<br>
  [Tai's Assets](https://assetstore.unity.com/publishers/17505)<br>
  [DOTween](https://dotween.demigiant.com/)<br>
  [UI Modal](https://assetstore.unity.com/packages/tools/gui/ui-modal-175169)<br>
  [冰焰模板](https://chinadlrs.com/app/?id=41)<br>
# TODO
- [x] Player重力实现
- [ ] 皮肤系统
- [x] 重写引导线
<html>
  <h1>ChangeLogs</h1>
  <h2>2025/4/16 Update Version 1.1.2</h2>
  1.重写引导线生成<br>
  2.实现Player重力<br>
  3.修复一些小bug<br>
  <h2>2025/4/14 Update Version 1.0</h2>
  [功能基本完成]<br>
  1.修复部分情况下无法复活的bug<br>
  2.修复Player速度问题<br>
  <h2>2025/4/13</h2>
  1.修复雾气恢复<br>
  2.新增TimeLine恢复<br>
  3.新增GuidanceManager,GuidanceToggle<br>
  4.优化了DiamondEffect表现<br>
  5.新增TriggerRotatePlayer<br>
  6.新增Gravity恢复<br>
  7.修复若干bug<br>

  <h2>2025/4/11</h2>
  1.修改Player死亡逻辑<br>
  2.修复字体缺失问题<br>
  3.Pyramid接入IResettableManager<br>
  4.新增FogTrigger,FogManager接入IResettable<br>
  5.修改SetActive,并接入IResettable<br>
  6.CameraFollower接入IResettable<br>
  7.新增BackgroundManager并接入IResettable,BackgroundTrigger<br>
  8.修复了仅第一个检查点能复活的bug<br>
  <h2>2025/4/10</h2>
  1.新增可触发的引导框<br>
  2.新增引导框出现逻辑<br>
  3.新增引导线出现逻辑<br>
  4.修复了Respawn界面百分比结算动画失效的问题<br>
  5.新增ResettableManager类管理所有继承IResettable的实例<br>
  6.修改了Over和Respawn界面的视觉效果<br>
  7.新增FogManager管理场景雾气<br>
  8.新增拓展方法类<br>
  9.碎碎念更新(划掉<br>
  <h2>2025/4/9</h2>
  1.修改AudioManager中FadeoutLevelSoundtrack的逻辑防止冲突<br>
  2.修复复活时所有检查点标志都会变灰的问题<br>
  3.InLevelDebugLable中新增"当前SoundtrackTime"显示<br>
  4.重新开始时淡出的音乐不会立即消失<br>
  5.修复拾取皇冠后暂停再开始检查点标志会变灰的问题<br>
  6.偷点懒直接从冰焰模板(见"支持")里拖了CameraFollower,CameraShakeTrigger,CameraTrigger，SetActive,嘻嘻<br>
  7.新增复活后Soundtrack属性恢复<br>
  <h2>2025/4/8</h2>
  1.新增RespawnManager和RespawnAttributes以及修改了部分代码实现复活功能<br>
  2.修复<br>
  (1)有时相机无法回到正常位置的bug<br>
  (2)从暂停时重新开始游戏会清除生成物的bug
  <h3>目前已支持:</h3>
  (1)恢复Player位置<br>
  (2)恢复CameraController位置<br>
  (3)复活遮罩动画<br>
  (4)清空LevelProgressManager中的已收集物<br>
  (5)复活后检查点标志动画<br>
  (6)复活后生成ReadyInterface<br>
  (7)检查点自动记录属性<br>
  
  <h2>2025/4/2</h2>
  1.新增UnityEventTrigger<br>
  2.新增Pyramid<br>
  3.修复了Respawn界面调用基类ExitInterface出现空引用的问题<br>
  4.新增Drowned死亡模式<br>
  <h2>2025/4/1</h2>
  1.增加拾取检查点后死亡会先调用Respawn界面的逻辑<br>
  2.修复Respawn界面不能正确显示游玩进度的bug<br>
  3.新增玩家在不同死亡模式下会播放不同音频的逻辑<br>
  4.暴力修正了游戏结束时仍然能单次触发暂停的bug(GameController.cs - 339)<br>
  5.新增PyramidTrigger<br>
  <h2>2025/3/30</h2>
 1.Player新增AutoPlay功能<br>
 2.Player,CameraController,HintBoxTrigger内增加绘制Gizmos功能
</html>
