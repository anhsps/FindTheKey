using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player14 : MonoBehaviour
{
    private int starCount;
    private bool waiting;
    private bool isWin, isLose;
    private Vector3 gridPos;
    private Button UpBtn, DownBtn, LeftBtn, RightBtn;
    private float duration = 0.25f;

    // Start is called before the first frame update
    void Start()
    {
        gridPos = transform.position;

        UpBtn = GameObject.Find("UpBtn").GetComponent<Button>();
        DownBtn = GameObject.Find("DownBtn").GetComponent<Button>();
        LeftBtn = GameObject.Find("LeftBtn").GetComponent<Button>();
        RightBtn = GameObject.Find("RightBtn").GetComponent<Button>();

        UpBtn.onClick.AddListener(() => UpdatePos(Vector3.up));
        DownBtn.onClick.AddListener(() => UpdatePos(Vector3.down));
        LeftBtn.onClick.AddListener(() => UpdatePos(Vector3.left));
        RightBtn.onClick.AddListener(() => UpdatePos(Vector3.right));
    }

    // Update is called once per frame
    void Update()
    {
        MoveInput();
    }

    private void MoveInput()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow)) UpdatePos(Vector3.up);
        if (Input.GetKeyDown(KeyCode.DownArrow)) UpdatePos(Vector3.down);
        if (Input.GetKeyDown(KeyCode.LeftArrow)) UpdatePos(Vector3.left);
        if (Input.GetKeyDown(KeyCode.RightArrow)) UpdatePos(Vector3.right);
    }

    private void UpdatePos(Vector3 direction)
    {
        if (waiting) return;

        Vector3 nextPos = gridPos + direction;

        while (!CheckWall(nextPos))
        {
            gridPos = nextPos;

            Collider2D col = Physics2D.OverlapPoint(nextPos);
            if (col && col.gameObject.CompareTag("Finish"))
            {
                isWin = true;
                break;
            }
            else if (!LimitScreen(nextPos))
            {
                isLose = true;
                break;
            }
            else nextPos = gridPos + direction;
        }

        if (gridPos != transform.position)
        {
            StartCoroutine(WaitForChanges());
            StartCoroutine(Animate(gridPos));
            if (isWin) Invoke(nameof(CheckWin), duration * 2);
            if (isLose) Invoke(nameof(CheckLose), duration * 2);
            CheckBox(gridPos + direction);
        }
    }

    private void CheckBox(Vector3 nextPos)
    {
        Collider2D col = Physics2D.OverlapPoint(nextPos, LayerMask.GetMask("Box"));
        if (col) StartCoroutine(Fade(col.gameObject));
    }

    private void CheckWin() => GameManager14.Instance.GameWin();

    private void CheckLose()
    {
        waiting = true;
        GameManager14.Instance.Retry();
    }

    private bool CheckWall(Vector2 nextPos)
    {
        return Physics2D.OverlapPoint(nextPos, LayerMask.GetMask("Ground", "Box"));
    }

    private bool LimitScreen(Vector3 pos)
    {
        // convert pos: world space -> viewpost space
        Vector3 screenPos = Camera.main.WorldToViewportPoint(pos);
        // check pos vs range [0,1] trong Viewpost space
        return screenPos.x >= 0 && screenPos.x <= 1 && screenPos.y >= 0 && screenPos.y <= 1;
    }

    private IEnumerator WaitForChanges()
    {
        SoundManager14.Instance.PlaySound(4);
        waiting = true;
        yield return new WaitForSeconds(duration);
        waiting = false;
    }

    private IEnumerator Animate(Vector3 to)
    {
        float elapsed = 0;
        Vector3 from = transform.position;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = to;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Star"))
        {
            SoundManager14.Instance.PlaySound(5);
            GameManager14.Instance.UpdateStar();
            Destroy(collision.gameObject);
            HandlerKey.Instance.ActiveKey();
        }

        if (collision.gameObject.CompareTag("Key"))
        {
            var _key = collision.gameObject.GetComponent<ItemID>();
            if (_key)
            {
                SoundManager14.Instance.PlaySound(5);
                int keyID = _key.id;
                Destroy(collision.gameObject);
                DestroyLock(keyID);
            }
        }
    }

    private void DestroyLock(int keyID)
    {
        GameObject[] locks = GameObject.FindGameObjectsWithTag("Lock");
        foreach (GameObject lockObj in locks)
        {
            var _lock = lockObj.GetComponent<ItemID>();
            if (_lock && _lock.id == keyID)
            {
                SoundManager14.Instance.PlaySound(6);
                StartCoroutine(Fade(lockObj));
                break;
            }
        }
    }

    private IEnumerator Fade(GameObject obj)
    {
        yield return new WaitForSeconds(duration);

        float elapsed = 0;
        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        Color color = sr.color;

        while (elapsed < duration)
        {
            color.a = Mathf.Lerp(1f, 0, elapsed / (2 * duration));
            sr.color = color;
            elapsed += Time.deltaTime;
            yield return null;
        }
        color.a = 0;
        sr.color = color;
        Destroy(obj);
    }
}
