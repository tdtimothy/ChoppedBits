using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DialogueController : MonoBehaviour
{
    public Sprite[] characters;
    private Dialogue currentLine;
    public List<Dialogue> publicScript; 
    private Queue<Dialogue> script;
    public Image characterModel1;
    public Text characterName1;
    public Text displayedDialogue;
    // Start is called before the first frame update
    void Start()
    {
        script = new Queue<Dialogue>(publicScript);
        Dialogue currentLine = script.Dequeue();
        characterModel1.sprite = characters[currentLine.characterSprite];
        characterName1.text = currentLine.characterName;
        displayedDialogue.text = currentLine.line;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            if(script.Count == 0)
            {
                SceneManager.LoadScene("Puzzle0-1", LoadSceneMode.Single);
                return;
            }
            Dialogue currentLine = script.Dequeue();
            characterModel1.sprite = characters[currentLine.characterSprite];
            characterName1.text = currentLine.characterName;
            displayedDialogue.text = currentLine.line;
        }
    }
}
[System.Serializable]
public class Dialogue
{
    public int characterSprite;
    public string characterName;
    public string line;
    public int side;
    public Dialogue(int character, string characterName, string line) {
        this.characterSprite = character;
        this.characterName = characterName;
        this.line = line;
        side = 0;
    }
}
