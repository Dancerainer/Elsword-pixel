using System;
namespace ElswordPixel.CargoAirControlledPrivateV21 {
 // Byte-exact counterpart of CargoAirPaletteV21.shader's nearest LUT operation.
 // Index0/B!=255 preserves original RGB. Original alpha is always returned.
 public static class CargoAirPaletteLookupV21 {
  public static bool IsExactAir(string npc){return npc=="NUI_NASOD_MINER_THIN_AIR"||npc=="NUI_NASOD_MINER_FAT_B_AIR"||npc=="NUI_NASOD_MINER_FAT_C_AIR";}
  public static int PaletteOffset(byte low,byte high){return ((255-high)*256+low)*4;}
  public static void MapPixel(byte[] original,int p,byte[] index,byte[] palette,byte[] output){
   if(original==null||index==null||palette==null||output==null||palette.Length!=256*256*4||original.Length!=index.Length||output.Length!=original.Length||p<0||p+3>=original.Length)throw new ArgumentException("Invalid AIR palette input");
   int q=PaletteOffset(index[p],index[p+1]);bool mapped=original[p+3]!=0&&index[p+2]==255&&(index[p]!=0||index[p+1]!=0);
   output[p]=mapped?palette[q]:original[p];output[p+1]=mapped?palette[q+1]:original[p+1];output[p+2]=mapped?palette[q+2]:original[p+2];output[p+3]=original[p+3];
  }
  public static byte[] Render(byte[] original,byte[] index,byte[] palette){var result=new byte[original.Length];for(int p=0;p<original.Length;p+=4)MapPixel(original,p,index,palette,result);return result;}
 }
}
