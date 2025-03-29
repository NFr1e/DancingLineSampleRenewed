namespace Gravitons.UI.Modal
{
    using UnityEngine.UI;
    using UnityEngine;

    /// <summary>
    /// Manages the UI in the demo scene
    /// </summary>
    public class DemoManager : MonoBehaviour
    {
        public Button button;
        public Button button2;
        public Image image;

        private void Start()
        {
            button.onClick.AddListener(ShowModal);
            button2.onClick.AddListener(ShowModalWithCallback);
        }

        /// <summary>
        /// Show a simple modal
        /// </summary>
        private void ShowModal()
        {
            ModalManager.Show("模态标题", "详细内容", new[] { new ModalButton() { Text = "确定" } });
        }

        /// <summary>
        /// Shows a modal with callback
        /// </summary>
        private void ShowModalWithCallback()
        {
            ModalManager.Show("你好", "我是一个交互盒!",
                new[]
                {
                    new ModalButton()
                    {
                        Text = "确定",
                        Callback = () =>
                        {
                            ShowModalWithCallback();
                        }
                    },
                    new ModalButton()
                    {
                        Text = "关闭"
                    },
                    new ModalButton()
                    {
                        Text = "你是什么?",
                        Callback = ()=> ModalManager.Show("啊?你没听清吗","我是一个交互盒啊",
                            new[]
                            {
                                new ModalButton()
                                {
                                    Text= "豪德",Callback = () => ModalManager.Show("我玩安东的", "看不懂，我玩安东的。兄弟！\r\n兄弟，我们上！\r\n我的钻头\r\n突破天际！\r\n天元突破！\r\n直达地基！\r\n兄弟，我们一起",
                                    new[]
                                    {
                                        new ModalButton()
                                        {
                                            Text = "我玩安东的",Callback = ()=>Application.Quit()
                                        }
                                    })
                                },
                                new ModalButton()
                                {
                                    Text = "返回",Callback = () => ShowModalWithCallback()
                                }
                            })
                    }
                });
        }

        /// <summary>
        /// Change background color to a random color
        /// </summary>
        private void ChangeColor()
        {
            image.color = new Color(Random.value, Random.value, Random.value);
        }

        private void OnDestroy()
        {
            button.onClick.RemoveListener(ShowModal);
            button2.onClick.RemoveListener(ShowModalWithCallback);
        }
    }
}