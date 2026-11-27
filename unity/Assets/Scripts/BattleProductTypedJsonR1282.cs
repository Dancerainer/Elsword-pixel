using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using System.Reflection;
using System.Text;
namespace ElswordPixel {
 // Exact twenty-two roots; existing Read9/Factory6/Mine212 and their callers remain unchanged.
 internal static class BattleProductTypedJsonR1282 {
  sealed class NumberToken { internal readonly string Text; internal NumberToken(string s){Text=s;} }
  sealed class DecimalParts { internal bool Negative; internal string Digits; internal BigInteger Exponent; }
  internal static T Read<T>(byte[] raw){
   Type type=typeof(T);
   if(type!=typeof(BattleProductPcmDocumentR1325)&&
      type!=typeof(BattleProductWorldLayerDocumentR1325)&&
      type!=typeof(OutskirtsVisualParticle)&&
      type!=typeof(BattleProductModelR1282)&&
      type!=typeof(BattleProductAuditR1282)&&
      type!=typeof(BattleProductRulesR1282)&&
      type!=typeof(BattleProductClocksR1282)&&
      type!=typeof(BattleProductProjectionR1282)&&
      type!=typeof(BattleProductSceneR1282)&&
      type!=typeof(BattleProductLibraryR1282)&&
      type!=typeof(BattleProductDeferredR1282)&&
      type!=typeof(BattleProductAudioR1282)&&
      type!=typeof(NpcEffectDocument)&&
      type!=typeof(NpcSoundDocument)&&
      type!=typeof(NpcMechanismDocument)&&
      type!=typeof(OutskirtsNpcVisualDocument)&&
      type!=typeof(OutskirtsNpcMeshManifest)&&
      type!=typeof(AlteraAirshipStateDocumentV20)&&
      type!=typeof(AirshipStatePayloadV20)&&
      type!=typeof(AirshipDirectorDocumentV20)&&
      type!=typeof(AirshipDirectorGeometryV20)&&
      type!=typeof(AirshipDirectorCameraTrackV20))
    throw new ArgumentException("Only twenty-two explicit Battle product data DTO roots are supported");
   string text=new UTF8Encoding(false,true).GetString(raw);
   return (T)Map(new Reader(text).Document(),type);
  }
  // Exact metadata header entry; shared runtime DTOs and the 22-root generic whitelist stay unchanged.
  internal static string ReadSourceRulesSha256R1341(byte[] raw){
   string text=new UTF8Encoding(false,true).GetString(raw);
   var header=(BattleProductSourceRulesHeaderR1341)Map(new Reader(text).Document(),typeof(BattleProductSourceRulesHeaderR1341));
   return header==null?null:header.sourceRulesSha256;
  }
  static bool PrivateDto(Type type){
   if(type.DeclaringType!=typeof(FactoryPreparedComponentV21)||!type.IsNestedPrivate||!type.IsSealed)return false;
   return type.Name=="ModelDocument"||type.Name=="SourceRoom"||type.Name=="RulesDocument"||
          type.Name=="ActivationDocument"||type.Name=="ActivationRoom"||type.Name=="ActivationBirth";
  }
  static bool NumericEnum(Type type){return type==typeof(CombatState)||type==typeof(HitReaction)||type==typeof(EncounterClearMode);}
  static void RequireSupportedType(Type type){
   if(type==typeof(string)||type==typeof(bool)||type==typeof(double)||type==typeof(float)||type==typeof(int)||type==typeof(long)||NumericEnum(type))return;
   if(type.IsArray&&type.GetArrayRank()==1){RequireSupportedType(type.GetElementType());return;}
   if(type.IsClass&&type.IsSealed&&!type.IsAbstract&&type.BaseType==typeof(object)&&
      type.Assembly==typeof(BattleProductTypedJsonR1282).Assembly&&type.Namespace=="ElswordPixel"&&
      (type.IsPublic||PrivateDto(type)))return;
   throw new FormatException("Unsupported Factory DTO field type: "+type.FullName);
  }
  static object Map(object value,Type type){
   RequireSupportedType(type);
   if(value==null){if(type.IsValueType)throw new FormatException("Null for a value field");return null;}
   if(type==typeof(string)){if(!(value is string))throw new FormatException("String field type");return value;}
   if(type==typeof(bool)){if(!(value is bool))throw new FormatException("Boolean field type");return value;}
   if(type==typeof(double)||type==typeof(float)||type==typeof(int)||type==typeof(long)){
    var number=value as NumberToken;if(number==null)throw new FormatException("Numeric field type");
    if(type==typeof(double))return Binary64(Parts(number.Text));
    if(type==typeof(float))return Binary32(Parts(number.Text));
    BigInteger integer=Integer(Parts(number.Text));
    if(type==typeof(int))return (int)integer;
    return (long)integer;
   }
   if(NumericEnum(type)){
    var number=value as NumberToken;if(number==null)throw new FormatException("Numeric enum field type");
    return Enum.ToObject(type,(int)Integer(Parts(number.Text)));
   }
   if(type.IsArray){
    var values=value as List<object>;if(values==null)throw new FormatException("Array field type");
    Type element=type.GetElementType();Array array=Array.CreateInstance(element,values.Count);
    for(int i=0;i<values.Count;i++)array.SetValue(Map(values[i],element),i);
    return array;
   }
   var obj=value as Dictionary<string,object>;
   if(obj==null||!type.IsClass||type.IsAbstract)throw new FormatException("DTO object field type");
   object result=PrivateDto(type)?Activator.CreateInstance(type,true):Activator.CreateInstance(type);
   foreach(var field in type.GetFields(BindingFlags.Public|BindingFlags.Instance)){
    object item;RequireSupportedType(field.FieldType);
    if(field.IsInitOnly)throw new FormatException("Readonly Factory DTO field: "+field.Name);
    // Unknown JSON members have already been fully parsed, then are ignored.
    // Missing fields retain the ordinary constructor and declared initializer values.
    if(obj.TryGetValue(field.Name,out item))field.SetValue(result,Map(item,field.FieldType));
   }
   return result;
  }
  static DecimalParts Parts(string token){
   int i=0;bool negative=token[0]=='-';if(negative)i++;
   var digits=new StringBuilder();int fraction=0;bool afterPoint=false;
   while(i<token.Length&&token[i]!='e'&&token[i]!='E'){
    char c=token[i++];if(c=='.'){afterPoint=true;continue;}
    digits.Append(c);if(afterPoint)fraction++;
   }
   BigInteger exponent=BigInteger.Zero;
   if(i<token.Length){
    i++;bool minus=false;if(token[i]=='+'||token[i]=='-'){minus=token[i]=='-';i++;}
    while(i<token.Length)exponent=exponent*10+(token[i++]-'0');
    if(minus)exponent=-exponent;
   }
   exponent-=fraction;
   string all=digits.ToString();int start=0;while(start<all.Length&&all[start]=='0')start++;
   if(start==all.Length)return new DecimalParts{Negative=negative,Digits="0",Exponent=BigInteger.Zero};
   int end=all.Length;while(end>start&&all[end-1]=='0'){end--;exponent++;}
   return new DecimalParts{Negative=negative,Digits=all.Substring(start,end-start),Exponent=exponent};
  }
  static BigInteger Coefficient(string digits){
   BigInteger n=BigInteger.Zero;foreach(char c in digits)n=n*10+(c-'0');return n;
  }
  static BigInteger Integer(DecimalParts p){
   if(p.Digits=="0")return BigInteger.Zero;
   // Trailing decimal zeroes were removed. A remaining negative exponent is fractional.
   if(p.Exponent.Sign<0)throw new FormatException("Fractional number for integer field");
   if(p.Exponent+p.Digits.Length-1>18)throw new OverflowException("Integer field range");
   BigInteger n=Coefficient(p.Digits)*BigInteger.Pow(10,(int)p.Exponent);return p.Negative?-n:n;
  }
  static int BitLength(BigInteger n){
   byte[] bytes=n.ToByteArray();int last=bytes.Length-1;while(last>0&&bytes[last]==0)last--;
   int bits=last*8;byte top=bytes[last];while(top!=0){bits++;top>>=1;}return bits;
  }
  static BigInteger Rounded(BigInteger n,BigInteger d,int shift){
   if(shift>=0)n<<=shift;else d<<=-shift;
   BigInteger remainder;BigInteger q=BigInteger.DivRem(n,d,out remainder);
   int halfway=(remainder<<1).CompareTo(d);
   if(halfway>0||(halfway==0&&!q.IsEven))q++;
   return q;
  }
  static double FromBits(ulong magnitude,bool negative){
   ulong bits=magnitude|(negative?0x8000000000000000UL:0UL);
   return BitConverter.Int64BitsToDouble(unchecked((long)bits));
  }
  static double Binary64(DecimalParts p){
   if(p.Digits=="0")return FromBits(0,p.Negative);
   BigInteger order=p.Exponent+p.Digits.Length-1;
   // These decimal orders are strictly beyond the binary64 rounding thresholds.
   if(order>308)return FromBits(0x7ff0000000000000UL,p.Negative);
   if(order< -324)return FromBits(0,p.Negative);
   BigInteger n=Coefficient(p.Digits),d=BigInteger.One;
   if(p.Exponent.Sign>=0)n*=BigInteger.Pow(10,(int)p.Exponent);
   else d=BigInteger.Pow(10,(int)(-p.Exponent));
   int e=BitLength(n)-BitLength(d);
   if(e>=0){if(n<(d<<e))e--;}else if((n<<-e)<d)e--;
   if(e< -1022){
    BigInteger q=Rounded(n,d,1074);
    // q==2^52 is the carry from largest subnormal to smallest normal.
    return FromBits((ulong)q,p.Negative);
   }
   if(e>1023)return FromBits(0x7ff0000000000000UL,p.Negative);
   BigInteger significand=Rounded(n,d,52-e);
   if(significand==(BigInteger.One<<53)){significand>>=1;e++;}
   if(e>1023)return FromBits(0x7ff0000000000000UL,p.Negative);
   ulong fraction=(ulong)(significand-(BigInteger.One<<52));
   return FromBits(((ulong)(e+1023)<<52)|fraction,p.Negative);
  }
  // Pure test surface: the exact same lexer and conversion, never a host decimal parser.
  internal static double Binary64Token(string token){
   var number=new Reader(token).Document() as NumberToken;
   if(number==null)throw new FormatException("A JSON number token is required");
   return Binary64(Parts(number.Text));
  }
  // R295: round the original decimal rational directly to binary32, never via double.
  static float FromBits32(uint magnitude,bool negative){
   uint bits=magnitude|(negative?0x80000000U:0U);
   return BitConverter.ToSingle(BitConverter.GetBytes(bits),0);
  }
  static float Binary32(DecimalParts p){
   if(p.Digits=="0")return FromBits32(0,p.Negative);
   BigInteger order=p.Exponent+p.Digits.Length-1;
   if(order>38)return FromBits32(0x7f800000U,p.Negative);
   if(order< -46)return FromBits32(0,p.Negative);
   BigInteger n=Coefficient(p.Digits),d=BigInteger.One;
   if(p.Exponent.Sign>=0)n*=BigInteger.Pow(10,(int)p.Exponent);
   else d=BigInteger.Pow(10,(int)(-p.Exponent));
   int e=BitLength(n)-BitLength(d);
   if(e>=0){if(n<(d<<e))e--;}else if((n<<-e)<d)e--;
   if(e< -126)return FromBits32((uint)Rounded(n,d,149),p.Negative);
   if(e>127)return FromBits32(0x7f800000U,p.Negative);
   BigInteger significand=Rounded(n,d,23-e);
   if(significand==(BigInteger.One<<24)){significand>>=1;e++;}
   if(e>127)return FromBits32(0x7f800000U,p.Negative);
   uint fraction=(uint)(significand-(BigInteger.One<<23));
   return FromBits32(((uint)(e+127)<<23)|fraction,p.Negative);
  }
  internal static float Binary32Token(string token){
   var number=new Reader(token).Document() as NumberToken;
   if(number==null)throw new FormatException("A JSON number token is required");
   return Binary32(Parts(number.Text));
  }
  sealed class Reader {
   readonly string text;int at;
   internal Reader(string value){if(value==null)throw new ArgumentNullException("value");text=value;}
   void Space(){while(at<text.Length&&(text[at]==' '||text[at]=='\t'||text[at]=='\r'||text[at]=='\n'))at++;}
   FormatException Bad(){return new FormatException("Invalid JSON at character "+at.ToString(CultureInfo.InvariantCulture));}
   internal object Document(){if(text.Length>0&&text[0]=='\ufeff')at++;Space();object value=Value();Space();if(at!=text.Length)throw Bad();return value;}
   object Value(){
    Space();if(at==text.Length)throw Bad();char c=text[at];
    if(c=='"')return String();if(c=='{')return Object();if(c=='[')return Array();
    if(c=='t'){Literal("true");return true;}if(c=='f'){Literal("false");return false;}if(c=='n'){Literal("null");return null;}
    if(c=='-'||(c>='0'&&c<='9'))return Number();throw Bad();
   }
   void Literal(string value){for(int i=0;i<value.Length;i++)if(at==text.Length||text[at++]!=value[i])throw Bad();}
   Dictionary<string,object> Object(){
    at++;Space();var result=new Dictionary<string,object>(StringComparer.Ordinal);
    if(at<text.Length&&text[at]=='}'){at++;return result;}
    while(true){
     Space();if(at==text.Length||text[at]!='"')throw Bad();string key=String();Space();
     if(at==text.Length||text[at++]!=':')throw Bad();object value=Value();
     if(result.ContainsKey(key))throw new FormatException("Duplicate JSON member "+key);result.Add(key,value);Space();
     if(at==text.Length)throw Bad();char next=text[at++];if(next=='}')return result;if(next!=',')throw Bad();
    }
   }
   List<object> Array(){
    at++;Space();var result=new List<object>();if(at<text.Length&&text[at]==']'){at++;return result;}
    while(true){result.Add(Value());Space();if(at==text.Length)throw Bad();char next=text[at++];if(next==']')return result;if(next!=',')throw Bad();}
   }
   string String(){
    at++;var result=new StringBuilder();
    while(at<text.Length){
     char c=text[at++];if(c=='"')return result.ToString();if(c<0x20)throw Bad();
     if(c!='\\'){result.Append(c);continue;}
     if(at==text.Length)throw Bad();c=text[at++];
     switch(c){
      case '"':case '\\':case '/':result.Append(c);break;
      case 'b':result.Append('\b');break;case 'f':result.Append('\f');break;case 'n':result.Append('\n');break;case 'r':result.Append('\r');break;case 't':result.Append('\t');break;
      case 'u':
       int value=0;for(int i=0;i<4;i++){if(at==text.Length)throw Bad();char h=text[at++];int digit=h>='0'&&h<='9'?h-'0':h>='a'&&h<='f'?h-'a'+10:h>='A'&&h<='F'?h-'A'+10:-1;if(digit<0)throw Bad();value=value*16+digit;}result.Append((char)value);break;
      default:throw Bad();
     }
    }
    throw Bad();
   }
   NumberToken Number(){
    int start=at;if(text[at]=='-'){at++;if(at==text.Length)throw Bad();}
    if(text[at]=='0')at++;
    else{if(text[at]<'1'||text[at]>'9')throw Bad();while(at<text.Length&&text[at]>='0'&&text[at]<='9')at++;}
    if(at<text.Length&&text[at]=='.'){at++;int first=at;while(at<text.Length&&text[at]>='0'&&text[at]<='9')at++;if(at==first)throw Bad();}
    if(at<text.Length&&(text[at]=='e'||text[at]=='E')){at++;if(at<text.Length&&(text[at]=='+'||text[at]=='-'))at++;int first=at;while(at<text.Length&&text[at]>='0'&&text[at]<='9')at++;if(at==first)throw Bad();}
    return new NumberToken(text.Substring(start,at-start));
   }
  }
 }
}
