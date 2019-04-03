using System;

namespace Swift
{
    /// <summary>
    /// 根据基本的鼠标按下，弹起，移动事件，构建更抽象的操作事件
    /// </summary>
    public class PointerOpsHandler
    {
        // 当前状态
        int currentState = 0; // 0-default, 1-down, 2-pressing, 3-dragging

        public Action OnClicked;

        public void Down()
        {
            switch (currentState)
            {
                case 0:
                    currentState = 1;
                    break;
            }
        }

        public void Up()
        {
            switch (currentState)
            {
                case 1:
                    currentState = 0;
                    OnClicked.SC();
                    break;
            }
        }
    }
}
