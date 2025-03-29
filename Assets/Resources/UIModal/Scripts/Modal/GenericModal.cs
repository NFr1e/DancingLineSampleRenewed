using DG.Tweening;
namespace Gravitons.UI.Modal
{
    using System.ComponentModel;
    using UnityEngine;
    using UnityEngine.UI;

    public class GenericModal : Modal
    {
        [Tooltip("Modal title")]
        [SerializeField] protected Text m_Title;
        [Tooltip("Modal body")]
        [SerializeField] protected Text m_Body;
        [Tooltip("Buttons in the modal")]
        [SerializeField] protected Button[] m_Buttons;
        private Tween ScaleTweenIn,ScaleTweenOut, alphaTweenIn,alphaTweenOut,PosTweenIn,PosTweenOut;
        /// <summary>
        /// Deactivate buttons in awake
        /// </summary>
        public void Awake()
        {
            for (int i = 0; i < m_Buttons.Length; i++)
            {
                m_Buttons[i].gameObject.SetActive(false);
            }
        }
        public override void Show(ModalContentBase modalContent, ModalButton[] modalButton)
        {

            GenericModalContent content = (GenericModalContent) modalContent;
            m_Title.text = content.Title;
            m_Body.text = content.Body;
            //Activate buttons and populate properties
            for (int i = 0; i < modalButton.Length; i++)
            {
                if (i >= m_Buttons.Length)
                {
                    Debug.LogError($"Maximum number of buttons of this modal is {m_Buttons.Length}. But {modalButton.Length} ModalButton was given. To display all buttons increase the size of the button array to at least {modalButton.Length}");
                    return;
                }
                m_Buttons[i].gameObject.SetActive(true);
                m_Buttons[i].GetComponentInChildren<Text>().text = modalButton[i].Text;
                int index = i; //Closure
                m_Buttons[i].onClick.AddListener(() =>
                {
                    if (modalButton[index].Callback != null)
                    {
                        modalButton[index].Callback();
                    }
                    
                    if (modalButton[index].CloseModalOnClick)
                    {
                        Animate(2);
                    }
                    m_Buttons[index].onClick.RemoveAllListeners();
                });
            }

            Animate();
        }

        //mode1 =>in mode2 => out
        public void Animate(int mode = 1)
        {
            CanvasGroup group;
            if (!GetComponent<CanvasGroup>()) gameObject.AddComponent<CanvasGroup>();
            group = GetComponent<CanvasGroup>();
            
            Vector3 originScale = transform.localScale;
            Vector3 originPos = transform.localPosition;
            Vector3 editedScale = gameObject.transform.localScale * 0.6f;
            Vector3 editedPos = new Vector3(originPos.x, originPos.y + 100, originPos.z);

            switch (mode) 
            {
                case 1:
                    if (ScaleTweenIn != null) ScaleTweenIn.Kill();
                    if (alphaTweenIn != null) alphaTweenIn.Kill();
                    if(PosTweenIn != null) PosTweenIn.Kill();

                    group.alpha = 0;
                    group.interactable = false;

                    transform.localScale = editedScale;
                    transform.localPosition = editedPos;

                    ScaleTweenIn = gameObject.transform.DOScale(originScale, 0.8f).SetEase(Ease.OutExpo);
                    PosTweenIn = gameObject.transform.DOLocalMove(originPos, 0.5f).SetEase(Ease.OutExpo);

                    alphaTweenIn = DOTween.To(() => group.alpha, a => group.alpha = a, 1, 0.3f).SetEase(Ease.OutExpo).OnComplete(() =>
                    {
                        group.interactable = true;
                        group.alpha = 1;
                    });
                    break;
                case 2:
                    if (ScaleTweenOut != null) ScaleTweenOut.Kill();
                    if (alphaTweenOut != null) alphaTweenOut.Kill();
                    if (ScaleTweenIn != null) ScaleTweenIn.Kill();
                    if (alphaTweenIn != null) alphaTweenIn.Kill();
                    if(PosTweenIn != null) PosTweenIn.Kill();
                    if(PosTweenOut != null) PosTweenOut.Kill();

                    group.alpha = 1;
                    group.interactable = false;

                    transform.localScale = originScale;

                    ScaleTweenOut = transform.DOScale(editedScale, 0.5f).SetEase(Ease.OutExpo);
                    PosTweenOut = transform.DOLocalMove(editedPos, 0.3f).SetEase(Ease.OutSine);
                    alphaTweenOut = DOTween.To(() => group.alpha, a => group.alpha = a, 0, 0.2f).SetEase(Ease.OutExpo).OnComplete(() => 
                    {
                        Close();
                    });
                    break;
            }
        }
    }
}