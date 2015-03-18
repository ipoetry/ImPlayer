using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Player.HotKey
{
    /// <summary>
    /// 热键设置控件
    /// </summary>
    [TemplatePartAttribute(Name = "PART_HotKeyText", Type = typeof(TextBox))]
    //[TemplatePartAttribute(Name = "PART_Clear", Type = typeof(Button))]
    public class HotKeySettingControl : ContentControl
    {
        /// <summary>
        /// 标识Command依赖项属性
        /// </summary>
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(Player.PlayController.Commands), typeof(HotKeySettingControl), new PropertyMetadata(Player.PlayController.Commands.None));
        /// <summary>
        /// 标识HotKey依赖项属性
        /// </summary>
        public static readonly DependencyProperty HotKeyProperty = DependencyProperty.Register("HotKey", typeof(HotKey), typeof(HotKeySettingControl));
        /// <summary>
        /// 标识HotKeyText依赖项属性
        /// </summary>
        public static readonly DependencyProperty HotKeyTextProperty = DependencyProperty.Register("HotKeyText", typeof(string), typeof(HotKeySettingControl));

        /// <summary>
        /// 热键将执行的命令
        /// </summary>
        public Player.PlayController.Commands Command
        {
            get { return (Player.PlayController.Commands)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        /// <summary>
        /// 触发命令的热键
        /// </summary>
        public HotKey HotKey
        {
            get { return (HotKey)GetValue(HotKeyProperty); }
            set
            {
                SetValue(HotKeyProperty, value);
                HotKeyText = HotKey == null ? null : HotKey.ToString();
            }
        }

        /// <summary>
        /// 热键相应的文本
        /// </summary>
        public string HotKeyText
        {
            get { return (string)GetValue(HotKeyTextProperty); }
            private set
            {
                SetValue(HotKeyTextProperty, value);
                TextBox hotKeyText = this.Template.FindName("PART_HotKeyText", this) as TextBox;
                if (hotKeyText != null)
                    hotKeyText.Text = HotKeyText;
            }
        }

        /// <summary>
        /// 将System.Windows.Input.Key与HotKey.ControlKeys关联起来的字典
        /// </summary>
        private static Dictionary<Key, HotKey.ControlKeys> keyMap;
        /// <summary>
        /// 忽略的按键
        /// </summary>
        private static HashSet<Key> ignoredKeys = new HashSet<Key> { Key.None, Key.LineFeed, Key.KanaMode, Key.HangulMode, Key.JunjaMode, Key.FinalMode, Key.HanjaMode, Key.KanjiMode, Key.ImeConvert, Key.ImeNonConvert, Key.ImeAccept, Key.ImeModeChange, Key.ImeProcessed, Key.System, Key.NoName, Key.DeadCharProcessed };

        static HotKeySettingControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HotKeySettingControl), new FrameworkPropertyMetadata(typeof(HotKeySettingControl)));

            keyMap = new Dictionary<Key, HotKey.ControlKeys>();
            keyMap.Add(Key.LeftCtrl, HotKey.ControlKeys.Ctrl);
            keyMap.Add(Key.RightCtrl, HotKey.ControlKeys.Ctrl);
            keyMap.Add(Key.LeftAlt, HotKey.ControlKeys.Alt);
            keyMap.Add(Key.RightAlt, HotKey.ControlKeys.Alt);
            keyMap.Add(Key.LeftShift, HotKey.ControlKeys.Shift);
            keyMap.Add(Key.RightShift, HotKey.ControlKeys.Shift);
            keyMap.Add(Key.LWin, HotKey.ControlKeys.Win);
            keyMap.Add(Key.RWin, HotKey.ControlKeys.Win);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            TextBox hotKeyText = this.Template.FindName("PART_HotKeyText", this) as TextBox;
            if (hotKeyText != null)
            {
                hotKeyText.Text = HotKeyText;

                hotKeyText.PreviewKeyDown += new KeyEventHandler((sender, e) =>
                {
                    HotKey.ControlKeys control = HotKey.ControlKeys.None;
                    Key key = Key.None;

                    foreach (Key kkey in Enum.GetValues(typeof(Key)))
                    {
                        if (!ignoredKeys.Contains(kkey))
                        {
                            if (Keyboard.IsKeyDown(kkey))
                            {
                                if (keyMap.ContainsKey(kkey))
                                {
                                    control |= keyMap[kkey];
                                }
                                else
                                {
                                    key = kkey;
                                }
                            }
                        }
                    }
                   if (key.ToString() == "Back")
                    {
                        HotKey = null;
                        hotKeyText.Text = "无";
                    }
                    else
                    {
                        HotKey = new HotKey(control, key);
                    }
					e.Handled = true;
                });
            }
        }
    }
}
