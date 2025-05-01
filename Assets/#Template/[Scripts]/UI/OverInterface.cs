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

        public Image 
            CrownFill_1,
            CrownFill_2,
            CrownFill_3;

        private LevelData _levelData;
        private LevelProgressManager.LevelProgress _progress;
        private Tween 
            _barTween,
            _restartTween,
            _backTween;

        private Vector3 
            _resatrtOrigin,
            _backOrigin;
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

            _resatrtOrigin = RestartButton.transform.position;
            _backOrigin = ExitTrigger.transform.position;

            _restartTween?.Kill();
            _backTween?.Kill();

            RestartButton.transform.position = new(_resatrtOrigin.x,_resatrtOrigin.y - 200f,_resatrtOrigin.z);
            _restartTween = RestartButton.transform
                .DOMoveY(_resatrtOrigin.y, 0.5f)
                .SetEase(Ease.OutSine);
            ExitTrigger.transform.position = new(_backOrigin.x,_backOrigin.y - 200f,_resatrtOrigin.z);
            _backTween = ExitTrigger.transform
                .DOMoveY(_backOrigin.y, 0.5f)
                .SetEase(Ease.OutSine);

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

            _restartTween?.Kill();
            _backTween?.Kill();

            _restartTween = RestartButton.transform
                .DOMoveY(_resatrtOrigin.y - 400, 0.5f)
                .SetEase(Ease.OutSine);
            _backTween = ExitTrigger.transform
                .DOMoveY(_backOrigin.y - 400, 0.5f)
                .SetEase(Ease.OutSine);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }
        private void HandleInfoDisplay()
        {
            if (!PercentageBar || !NameTitleText || !PercentageText || !DiamondText || !_levelData) return;
            if (GameController.curGameState != GameState.Over) return;

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

            if (!CrownFill_1 || !CrownFill_2 || !CrownFill_3) return;

            CrownFill_1.enabled = _progress.CheckpointCount >= 1;
            CrownFill_2.enabled = _progress.CheckpointCount >= 2;
            CrownFill_3.enabled = _progress.CheckpointCount >= 3;

        }
    }
}