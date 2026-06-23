using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DecisionUI : MonoBehaviour
{
    [SerializeField] private GameObject decisionPanel;

    [SerializeField] private TMP_Text questionText;

    [SerializeField] private Button optionButton1;
    [SerializeField] private Button optionButton2;
    [SerializeField] private Button optionButton3;

    [SerializeField] private TMP_Text optionText1;
    [SerializeField] private TMP_Text optionText2;
    [SerializeField] private TMP_Text optionText3;

    [SerializeField] private Image timerFillImage;

    public GameObject DecisionPanel => decisionPanel;

    public TMP_Text QuestionText => questionText;

    public Button OptionButton1 => optionButton1;
    public Button OptionButton2 => optionButton2;
    public Button OptionButton3 => optionButton3;

    public TMP_Text OptionText1 => optionText1;
    public TMP_Text OptionText2 => optionText2;
    public TMP_Text OptionText3 => optionText3;

    public Image TimerFillImage => timerFillImage;
}