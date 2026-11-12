
using System.Collections;
using UnityEngine;

public class doorexplode : MonoBehaviour
{
    [SerializeField] private Transform objectToMove;
    [SerializeField] private GameObject objectToEnable;
    [SerializeField] private Vector3 movementAmount = new Vector3(0f, 2f, 0f);
    [SerializeField] private float movementTime = 0.25f;
    [SerializeField] private float activeDuration = 2f;
    [SerializeField] private float inactiveDuration = 2f;

    private void Start()
    {
        Transform target = objectToMove != null ? objectToMove : transform;

        if (objectToEnable == null)
        {
            Debug.LogError("Assign the object to enable!", this);
            return;
        }

        StartCoroutine(MoveAndToggle(target));
    }

    private IEnumerator MoveAndToggle(Transform target)
    {
        Vector3 originalPosition = target.localPosition;
        Vector3 movedPosition = originalPosition + movementAmount;

        while (true)
        {
            objectToEnable.SetActive(true);
            Coroutine moveUp = StartCoroutine(MoveTo(target, originalPosition, movedPosition));
            yield return new WaitForSeconds(activeDuration);
            yield return moveUp;

            objectToEnable.SetActive(false);

            yield return MoveTo(target, movedPosition, originalPosition);

            yield return new WaitForSeconds(inactiveDuration);
        }
    }

    private IEnumerator MoveTo(Transform target, Vector3 start, Vector3 end)
    {
        Matrix4x4 startMatrix = Matrix4x4.TRS(start, target.localRotation, target.localScale);
        Matrix4x4 endMatrix = Matrix4x4.TRS(end, target.localRotation, target.localScale);

        if (movementTime <= 0f)
        {
            target.localPosition = endMatrix.GetPosition();
            yield break;
        }

        float elapsed = 0f;

        while (elapsed < movementTime)
        {
            elapsed += Time.deltaTime;

            float progress = Mathf.Clamp01(elapsed / movementTime);

            Matrix4x4 currentMatrix = InterpolateMatrix(startMatrix, endMatrix, progress);
            target.localPosition = currentMatrix.GetPosition();

            yield return null;
        }

        target.localPosition = endMatrix.GetPosition();
    }

    private static Matrix4x4 InterpolateMatrix(Matrix4x4 start, Matrix4x4 end, float progress)
    {
        Matrix4x4 result = new Matrix4x4();
        for (int row = 0; row < 4; row++)
        {
            for (int column = 0; column < 4; column++)
            {
                result[row, column] = Mathf.Lerp(start[row, column], end[row, column], progress);
            }
        }

        return result;
    }
}
