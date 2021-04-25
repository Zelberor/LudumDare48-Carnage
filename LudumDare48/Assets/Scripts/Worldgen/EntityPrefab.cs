using UnityEngine;

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

	public GameObject rollAndInstanceEntity() {
		GameObject entity = null;
		float rnd = Random.Range(0.0f, 1.0f);
		if (rnd <= prob && currentCount < maxEntities) {
			entity = GameObject.Instantiate<GameObject>(prefab, RoomTools.entityParent.transform);
			++currentCount;
		}
		return entity;
	}

	public void entityDestroyed() {
		--currentCount;
	}
}