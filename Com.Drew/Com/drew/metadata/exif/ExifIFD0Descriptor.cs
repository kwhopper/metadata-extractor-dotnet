/*
 * Copyright 2002-2015 Drew Noakes
 *
 *    Modified by Yakov Danilov <yakodani@gmail.com> for Imazen LLC (Ported from Java to C#)
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *
 *        http://www.apache.org/licenses/LICENSE-2.0
 *
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 *
 * More information about this project is available at:
 *
 *    https://drewnoakes.com/code/exif/
 *    https://github.com/drewnoakes/metadata-extractor
 */

using JetBrains.Annotations;

namespace Com.Drew.Metadata.Exif
{
    /// <summary>
    /// Provides human-readable string representations of tag values stored in a <see cref="ExifIfd0Directory"/>.
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class ExifIfd0Descriptor : ExifDescriptorBase<ExifIfd0Directory>
    {
        public ExifIfd0Descriptor([NotNull] ExifIfd0Directory directory)
            : base(directory)
        {
        }
    }
}