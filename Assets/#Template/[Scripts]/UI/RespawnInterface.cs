using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using DancingLineFanmade.Gameplay;

namespace DancingLineFanmade.UI
{
    public class RespawnInterface : InterfaceBase
    {
        public Button RespawnButton;
        public Slider PercentageBar;
        public Text NameTitleText;
        public Text PercentageText;
        public Text DiamondText;

        private bool _isRespawn = false;
        private LevelData _levelData;
        private LevelProgressManager.LevelProgress _progress;
        protected override void EnterInterface()
        {
            base.EnterInterface();

            _levelData = GameController.instance.CurrentLevelData;

            RespawnButton.onClick.AddListener(() =>
            {
                _isRespawn = true;
                ExitInterface();
            });

            UserInterfaceEvents.TriggerRespawnEnterEvent();
        }
        protected override void UpdateInterface()
        {
            base.UpdateInterface();

            HandleInfoDisplay();
        }
        public override void ExitInterface()
        {
            base.ExitInterface();

            if(!_isRespawn)
                UserInterfaceEvents.TriggerRespawnExitEvent();
        }
        protected override void OnEnable()
        {
            base.OnEnable();
        }
        private void HandleInfoDisplay()
        {
            if (!PercentageBar || !NameTitleText || !PercentageText || !DiamondText || !_levelData) return;

            _progress = LevelProgressManager.instance.currentProgress;

            NameTitleText.text = _levelData.LevelName;

            PercentageBar.maxValue = 100;
            PercentageBar.value = 0;
            DOTween.To(() => PercentageBar.value, a => PercentageBar.value = a, _progress.Percentage, 2f).SetEase(Ease.OutExpo);

            PercentageText.text = $"{_progress.Percentage}%";
            DiamondText.text = $"{_progress.DiamondCount}/{_levelData.MaxDiamondCount}";
        }
    }
}
