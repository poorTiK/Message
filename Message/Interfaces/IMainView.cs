namespace Message.Interfaces
{
    internal interface IMainView
    {
        void CloseWindow();

        void AnimatedResize(int h, int w);

        void SetOpacity(double opasity);
    }
}