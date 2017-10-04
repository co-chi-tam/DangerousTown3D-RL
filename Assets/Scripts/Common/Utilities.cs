using UnityEngine;
using System.Collections;

using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class Utilities {

	public static Vector3 ToV3Round2(this Vector3 value) {
		var result = new Vector3 (
			(float) Math.Round ((double) value.x, 2), 
			(float) Math.Round ((double) value.y, 2), 
			(float) Math.Round ((double) value.z, 2)
		);
		return result;
	}

	public static float Round2(this float value) {
		return (float) Math.Round ((double) value, 2);
	}

	public static Transform GetTransformWithTagInHierarchy(Transform root, string tag) {
		if (root == null || root.tag == tag) {
			return root;
		}

		return GetTransformWithTagInHierarchy(root.transform.parent, tag);
	}
	
	// Reference: http://unitygems.com/saving-data-1-remember-me/
	#region Serialization

	public static string Base64Serialize<T>(T instance) where T : class {
		if (instance != null) {
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			MemoryStream memoryStream = new MemoryStream();

			binaryFormatter.Serialize(memoryStream, instance);

			return Convert.ToBase64String(memoryStream.GetBuffer());
		}

		return null;
	}

	public static T Base64Deserialize<T>(string instance) where T : class {
		if (!string.IsNullOrEmpty(instance)) {
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(instance));

			return binaryFormatter.Deserialize(memoryStream) as T;
		}

		return null;
	}

	#endregion

}
