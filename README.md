# DancingLineTemplate-Renewed
A unity project aiming to make a template as the game DancingLine.

## 关于作者
[Bilibili个人页](https://space.bilibili.com/291841883?spm_id_from=333.1007.0.0)

## 关于项目
  Project created in 2025/3
  
<html>
  <h1>ChangeLogs</h1>
  <h2>2025/4/8</h2>
  1.新增RespawnManager和RespawnAttributes以及修改了部分代码实现复活功能
  <h3>目前已支持:</h3>
  (1)恢复Player位置<br>
  (2)恢复CameraController位置<br>
  (3)复活遮罩动画<br>
  (4)清空LevelProgressManager中的已收集物<br>
  (5)复活后检查点标志动画<br>
  (6)复活后生成ReadyInterface<br>
  (7)检查点自动记录属性<br>
  2.修复<br>
  (1)有时相机无法回到正常位置的bug<br>
  (2)从暂停时重新开始游戏会清除生成物的bug
  
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
