﻿using System.Collections.Generic;
using Microsoft.Recognizers.Text;
using Microsoft.Recognizers.Text.Sequence;
using Newtonsoft.Json;

namespace Microsoft.Bot.Builder.Dialogs.Adaptive.Recognizers
{
    /// <summary>
    /// Recognizes hashtag input.
    /// </summary>
    public class HashtagEntityRecognizer : TextEntityRecognizer
    {
        /// <summary>
        /// Class identifier.
        /// </summary>
        [JsonProperty("$kind")]
        public const string Kind = "Microsoft.HashtagEntityRecognizer";

        /// <summary>
        /// Initializes a new instance of the <see cref="HashtagEntityRecognizer"/> class.
        /// </summary>
        public HashtagEntityRecognizer()
        {
        }

        /// <summary>
        /// Hashtag recognizing implementation.
        /// </summary>
        /// <param name="text">Text to recognize.</param>
        /// <param name="culture"><see cref="Culture"/> to use.</param>
        /// <returns>The recognized <see cref="ModelResult"/> list.</returns>
        protected override List<ModelResult> Recognize(string text, string culture)
        {
            return SequenceRecognizer.RecognizeHashtag(text, culture);
        }
    }
}
