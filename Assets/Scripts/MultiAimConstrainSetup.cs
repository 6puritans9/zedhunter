using UnityEngine;
using UnityEngine.Animations.Rigging;

public class MultiAimConstraintSetup : MonoBehaviour
{
	public MultiAimConstraint multiAimConstraint;
	public Transform targetTransform;
	public Transform[] sourceTransforms;

	void Start()
	{
		if (multiAimConstraint != null && sourceTransforms.Length > 0)
		{
			// Constrained Object ����
			multiAimConstraint.data.constrainedObject = targetTransform;

			// Source Objects ����
			var sourceObjects = multiAimConstraint.data.sourceObjects;
			sourceObjects.Clear();
			foreach (var transform in sourceTransforms)
			{
				sourceObjects.Add(new WeightedTransform(transform, 1f));  // �� �ҽ� Ʈ�������� ����ġ ����
			}
			multiAimConstraint.data.sourceObjects = sourceObjects;
		}
	}
}
