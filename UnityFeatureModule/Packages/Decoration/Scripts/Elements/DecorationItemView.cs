namespace TheOneStudio.GameFeature.Decoration.Elements
{
    using DG.Tweening;
    using TMPro;
    using UnityEngine;

    public class DecorationItemView : MonoBehaviour
    {
        [SerializeField] private float           fillRateMin;
        [SerializeField] private float           fillRateMax;
        [SerializeField] private MeshRenderer    meshRenderer;
        [SerializeField] private Canvas          canvas;
        [SerializeField] private GameObject      progressHolder;
        [SerializeField] private TextMeshProUGUI progressTxt;
        [SerializeField] private RectTransform   attractor;

        public RectTransform Attractor => this.attractor;

        private static readonly int   FillRate         = Shader.PropertyToID("_FillRate");
        private static readonly int   OutColor         = Shader.PropertyToID("_Outcolor");
        private static readonly Color ActiveOutColor   = new Color(255, 223, 31, 255);
        private static readonly Color InActiveOutColor = new Color(0, 0, 0, 0);

        private Tween fillTween;
        private Tween txtTween;
        private bool  isScaleTweenReady = true;
        private int   currentCostLeft;

        public void DoBuildProgress(float progress, int costLeft)
        {
            var lastCost = this.currentCostLeft;
            this.currentCostLeft = costLeft;
            var material = this.meshRenderer.material;
            var endValue = this.fillRateMin + (this.fillRateMax - this.fillRateMin) * progress;

            this.fillTween?.Kill();
            this.fillTween = material.DOFloat(endValue, FillRate, 1.5f);

            this.txtTween?.Kill();
            this.txtTween = DoCurrencyText(this.progressTxt, "{0}", lastCost, costLeft, 2f).SetEase(Ease.Linear);

            material.SetColor(OutColor, progress >= 1 ? ActiveOutColor : InActiveOutColor);
            return;

            Tween DoCurrencyText(TMP_Text text, string format, int from, int to, float duration)
            {
                return DOTween.To(
                    setter: x =>
                    {
                        text.text = string.Format(format, (int)x);
                    },
                    startValue: from,
                    endValue: to,
                    duration: duration);
            }
        }

        public void ScaleAnim()
        {
            if (!this.isScaleTweenReady) return;
            this.isScaleTweenReady = false;
            this.transform.DOPunchScale(Vector3.one / 7, .25f, 1, .15f).OnComplete(() => this.isScaleTweenReady = true);
        }

        public void SetProgress(float progress, int costLeft)
        {
            this.progressTxt.text = $"{costLeft}";
            this.currentCostLeft  = costLeft;
            var material = this.meshRenderer.material;
            var endValue = this.fillRateMin + (this.fillRateMax - this.fillRateMin) * progress;
            material.SetFloat(FillRate, endValue);

            material.SetColor(OutColor, progress >= 1 ? ActiveOutColor : InActiveOutColor);
        }

        public void BindProgress(int cost, Vector3 angle)
        {
            this.progressTxt.text             = $"{cost}";
            this.currentCostLeft              = cost;
            this.canvas.transform.eulerAngles = angle;
        }

        public void ShowCost() { this.progressHolder.gameObject.SetActive(true); }
        public void HideCost() { this.progressHolder.gameObject.SetActive(false); }
    }
}