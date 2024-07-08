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
			// Constrained Object 설정
			multiAimConstraint.data.constrainedObject = targetTransform;

			// Source Objects 설정
			var sourceObjects = multiAimConstraint.data.sourceObjects;
			sourceObjects.Clear();
			foreach (var transform in sourceTransforms)
			{
				sourceObjects.Add(new WeightedTransform(transform, 1f));  // 각 소스 트랜스폼과 가중치 설정
			}
			multiAimConstraint.data.sourceObjects = sourceObjects;
		}
	}
}
