using System.Collections.Generic;
using Piglet.GLTF.Extensions;
using Piglet.Newtonsoft.Json;

namespace Piglet.GLTF.Schema
{
	/// <summary>
	/// A keyframe animation.
	/// </summary>
	public class Animation : GLTFChildOfRootProperty
	{
		/// <summary>
		/// An array of channels, each of which targets an animation's sampler at a
		/// node's property. Different channels of the same animation can't have equal
		/// targets.
		/// </summary>
		public List<AnimationChannel> Channels;

		/// <summary>
		/// An array of samplers that combines input and output accessors with an
		/// interpolation algorithm to define a keyframe graph (but not its target).
		/// </summary>
		public List<AnimationSampler> Samplers;

		public Animation()
		{
			Channels = new List<AnimationChannel>();
			Samplers = new List<AnimationSampler>();
		}

		public static Animation Deserialize(GLTFRoot root, JsonReader reader)
		{
			var animation = new Animation();

			while (reader.Read() && reader.TokenType == JsonToken.PropertyName)
			{
				var curProp = reader.Value.ToString();

				switch (curProp)
				{
					case "channels":
						animation.Channels = reader.ReadList(() => AnimationChannel.Deserialize(root, reader, animation));
						break;
					case "samplers":
						animation.Samplers = reader.ReadList(() => AnimationSampler.Deserialize(root, reader));
						break;
					default:
						animation.DefaultPropertyDeserializer(root, reader);
						break;
				}
			}

			return animation;
		}

		public override void Serialize(JsonWriter writer)
		{
			writer.WriteStartObject();

			writer.WritePropertyName("channels");
			writer.WriteStartArray();
			foreach (var channel in Channels)
			{
				channel.Serialize(writer);
			}
			writer.WriteEndArray();

			writer.WritePropertyName("samplers");
			writer.WriteStartArray();
			foreach (var sampler in Samplers)
			{
				sampler.Serialize(writer);
			}
			writer.WriteEndArray();

			base.Serialize(writer);

			writer.WriteEndObject();
		}
	}
}
