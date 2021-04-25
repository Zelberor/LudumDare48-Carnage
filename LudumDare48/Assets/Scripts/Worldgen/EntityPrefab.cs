using UnityEngine;
using System;

public class EntityPrefab {
	private GameObject prefab;
	private float prob = 0.0f;
	private int maxEntities;
	private int currentCount = 0;

	public EntityPrefab(GameObject prefab, float probability, int maxEntities) {
		this.prefab = prefab;
		this.prob = probability;
		this.maxEntities = maxEntities;
	}

	public GameObject getInstance() {
		if (currentCount < maxEntities) {
			++currentCount;
			return GameObject.Instantiate<GameObject>(prefab, RoomTools.entityParent.transform);
		}
		return null;
	}

	public void entityDestroyed() {
		--currentCount;
	}

	public float getProbability() {
		int countDiff = Math.Abs(maxEntities - currentCount);
		if (countDiff < maxEntities / 4) {
			float inverseCountDiff = maxEntities / 4 - countDiff;
			float normalizedCountDiff = inverseCountDiff / (float) (maxEntities / 4);
			float reducedProb = prob - normalizedCountDiff * prob;
			Debug.Log("Reduced Prob: " + reducedProb + " cur: " + currentCount + " max: " + maxEntities);
			return reducedProb;
		}
		return prob;
	}
}