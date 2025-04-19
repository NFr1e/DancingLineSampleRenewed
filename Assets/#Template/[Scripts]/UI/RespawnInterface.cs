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
        private Tween 
            _barTween,
            _respawnTween,
            _backTween;

        private Vector3
            _respawnOrigin,
            _backOrigin;

        protected override void EnterInterface()
        {
            base.EnterInterface();

            _levelData = GameController.instance.CurrentLevelData;

            RespawnButton.onClick.AddListener(() =>
            {
                _isRespawn = true;
                RespawnEvents.CallResapwan();
                ExitInterface();
            });

            UserInterfaceEvents.TriggerRespawnEnterEvent();

            _respawnOrigin = RespawnButton.transform.position;
            _backOrigin = ExitTrigger.transform.position;

            _respawnTween?.Kill();
            _backTween?.Kill();

            RespawnButton.transform.position = new(_respawnOrigin.x, _respawnOrigin.y - 200f, _respawnOrigin.z);
            _respawnTween = RespawnButton.transform
                .DOMoveY(_respawnOrigin.y, 0.5f)
                .SetEase(Ease.OutSine);
            ExitTrigger.transform.position = new(_backOrigin.x, _backOrigin.y - 200f, _respawnOrigin.z);
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

            if(!_isRespawn)
                UserInterfaceEvents.TriggerRespawnExitEvent();

            _respawnTween?.Kill();
            _backTween?.Kill();

            _respawnTween = RespawnButton.transform
                .DOMoveY(_respawnOrigin.y - 200f, 0.5f)
                .SetEase(Ease.OutSine);
            _backTween = ExitTrigger.transform
                .DOMoveY(_backOrigin.y - 200, 0.5f)
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
        }
    }
}
