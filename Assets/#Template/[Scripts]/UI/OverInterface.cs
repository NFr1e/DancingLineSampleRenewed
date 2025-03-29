using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using DancingLineFanmade.Gameplay;

namespace DancingLineFanmade.UI
{
    public class OverInterface : InterfaceBase
    {
        public Button RestartButton;
        public Slider PercentageBar;
        public Text NameTitleText;
        public Text PercentageText;
        public Text DiamondText;

        private LevelData _levelData;
        private LevelProgressManager.LevelProgress _progress;
        private Tween _barTween;
        protected override void EnterInterface()
        {
            base.EnterInterface();

            _levelData = GameController.instance.CurrentLevelData;

            RestartButton.onClick.AddListener(() =>
            {
                GameController.ReloadScene();
                ExitInterface();
            });

            UserInterfaceEvents.TriggerOverEnterEvent();
        }
        protected override void UpdateInterface()
        {
            base.UpdateInterface();

            HandleInfoDisplay();
        }
        public override void ExitInterface()
        {
            base.ExitInterface();

            UserInterfaceEvents.TriggerOverExitEvent();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }
        private void HandleInfoDisplay()
        {
            if (!PercentageBar || !NameTitleText || !PercentageText || !DiamondText || !_levelData) return;

            NameTitleText.text = _levelData.LevelName;

            _progress = LevelProgressManager.instance.currentProgress;

            if (_barTween == null)
            {
                PercentageBar.maxValue = 100;
                PercentageBar.value = 0;
                _barTween = DOTween.To(() => PercentageBar.value, a => PercentageBar.value = a, _progress.Percentage, 2f).SetEase(Ease.OutExpo).OnComplete(() => _barTween.Complete());
            }

            PercentageText.text = $"{_progress.Percentage}%";
            DiamondText.text = $"{_progress.DiamondCount}/{_levelData.MaxDiamondCount}";
        }
    }
}