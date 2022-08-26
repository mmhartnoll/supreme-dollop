using MindSculptor.App.MtgaOverlay.Models;
using MindSculptor.Tools.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MindSculptor.App.MtgaOverlay.DataTypes
{
    internal class MatchCardView : IComparable<MatchCardView>
    {
        public int MtgaCardId { get; }
        public string Name { get; }
        public IEnumerable<MatchCardFaceView> Faces { get; }
        public ColorIdentity ColorIdentity { get; }

        private MatchCardView(int mtgaCardId, string name, IEnumerable<MatchCardFaceView> faces, ColorIdentity colorIdentity)
        {
            MtgaCardId    = mtgaCardId;
            Name          = name;
            Faces         = faces;
            ColorIdentity = colorIdentity;
        }

        public static async Task<MatchCardView> LoadAsync(CardModel model)
        {
            var name          = await model.GetName().ConfigureAwait(false);
            var colorIdentity = await model.GetColorIdentity().ConfigureAwait(false);
            var faceModels    = await model.GetFacesAsync().ConfigureAwait(false);

            var faces = await faceModels.SelectAsync(faceModel => MatchCardFaceView.LoadAsync(faceModel))
                .ToListAsync()
                .ConfigureAwait(false);
            return new MatchCardView(model.MtgaCardId, name, faces.Enumerate(), colorIdentity);
        }

        public int CompareTo(MatchCardView other)
        {
            var isLand = Faces.SelectMany(face => face.MainTypes).Contains("Land");
            var otherIsLand = other.Faces.SelectMany(face => face.MainTypes).Contains("Land");

            if (isLand && !otherIsLand) return 1;
            if (!isLand && otherIsLand) return -1;

            var isColorlessArtifact = ColorIdentity == ColorIdentity.Colorless && Faces.SelectMany(face => face.MainTypes).Contains("Artifact");
            var otherIsColorlessArtifact = other.ColorIdentity == ColorIdentity.Colorless && other.Faces.SelectMany(face => face.MainTypes).Contains("Artifact");

            if (isColorlessArtifact && !otherIsColorlessArtifact) return 1;
            if (!isColorlessArtifact && otherIsColorlessArtifact) return -1;

            var mappedColorIdentity = ColorIdentityOrderMap(ColorIdentity);
            var otherMappedColorIdentity = ColorIdentityOrderMap(other.ColorIdentity);

            if (mappedColorIdentity != otherMappedColorIdentity)
                return mappedColorIdentity.CompareTo(otherMappedColorIdentity);

            return Name.CompareTo(other.Name);

            static int ColorIdentityOrderMap(ColorIdentity colorIdentity)
                => colorIdentity switch
                {
                    ColorIdentity.Colorless => 0,
                    ColorIdentity.White     => 1,
                    ColorIdentity.Blue      => 2,
                    ColorIdentity.Black     => 3,
                    ColorIdentity.Red       => 4,
                    ColorIdentity.Green     => 5,
                    _                       => 6
                };
        }
    }
}
