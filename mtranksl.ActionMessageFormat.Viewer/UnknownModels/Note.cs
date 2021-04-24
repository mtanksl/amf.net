﻿using mtanksl.ActionMessageFormat;
using System;
using System.Text;

namespace mtranksl.ActionMessageFormat.Viewer.UnknownModels
{
    [TraitClass("com.etcbase.sfc.model.note.Note")]
    public class Note : IExternalizable
    {
        public string UnknownBytes { get; set; }

        public void Write(AmfWriter writer)
        {
            throw new NotImplementedException();
        }

        public void Read(AmfReader reader)
        {
            var unknownBytes = new StringBuilder();

            for (int i = 0; i < 89; i++)
            {
                byte anything = reader.ReadByte();

                unknownBytes.AppendFormat("{0:X2} ", anything);
            }

            UnknownBytes = unknownBytes.ToString();
        }
    }
}