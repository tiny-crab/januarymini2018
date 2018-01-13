using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Rate : MonoBehaviour
{
    string file = "C:\\Users\\adams\\AppData\\Local\\Packages\\dba29248-bfd9-4c24-a091-e2d0f24b8767_jgkmswfchj13a\\LocalState\\data.txt";

    Text guiText;

    [SerializeField] float loseValue = 90;

    void Start()
    {
        guiText = GetComponent<Text>();
        StartCoroutine(GetText());
    }

    IEnumerator GetText()
    {
        while (true)
        {
            try
            {
                string text = System.IO.File.ReadAllText(file);

                // Display the file contents to the console. Variable text is a string.
                Debug.Log("Contents of data.txt " + text);
                if (text != "0") 
                    guiText.text = text;

                int value = int.Parse(text);
                if (value >= loseValue)
                {
                    SceneManager.LoadScene("YouLost");
                }
            }
            catch
            {

            }
            yield return new WaitForSeconds(1f);
        }


    }
}