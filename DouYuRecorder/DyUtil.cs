using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DySdk;
using System.Net;
using JsEngineChakra;

namespace DouYuRecorder
{
    class DyUtil
    {
        public static string DyGetRealRtmp(string url)
        {
            string jsondata = Function.GetRoomJsFromUrl(url);
            if (jsondata == null) return null;

            string CryptoJS = "var CryptoJS=CryptoJS||function(e,c){var s={},i=s.lib={},g=i.Base=function(){function b(){}return{extend:function(f){b.prototype=this;var h=new b;f&&h.mixIn(f);h.$super=this;return h},create:function(){var f=this.extend();f.init.apply(f,arguments);return f}," +
                "init:function(){},mixIn:function(f){for(var h in f){f.hasOwnProperty(h)&&(this[h]=f[h])}f.hasOwnProperty(\"toString\")&&(this.toString=f.toString)},clone:function(){return this.$super.extend(this)}}}(),u=i.WordArray=g.extend({init:function(b,f){b=this.words=b||[];this.sigBytes=f!=c?f:4*b.length}," +
                "toString:function(b){return(b||a).stringify(this)},concat:function(h){var j=this.words,l=h.words,k=this.sigBytes,h=h.sigBytes;this.clamp();if(k%4){for(var f=0;f<h;f++){j[k+f>>>2]|=(l[f>>>2]>>>24-8*(f%4)&255)<<24-8*((k+f)%4)}}else{if(65535<l.length){for(f=0;f<h;f+=4){j[k+f>>>2]=l[f>>>2]}}" +
                "else{j.push.apply(j,l)}}this.sigBytes+=h;return this},clamp:function(){var b=this.words,f=this.sigBytes;b[f>>>2]&=4294967295<<32-8*(f%4);b.length=e.ceil(f/4)},clone:function(){var b=g.clone.call(this);b.words=this.words.slice(0);return b}," +
                "random:function(b){for(var f=[],h=0;h<b;h+=4){f.push(4294967296*e.random()|0)}return u.create(f,b)}}),t=s.enc={},a=t.Hex={stringify:function(h){for(var j=h.words,h=h.sigBytes,l=[],k=0;k<h;k++){var f=j[k>>>2]>>>24-8*(k%4)&255;l.push((f>>>4).toString(16));l.push((f&15).toString(16))}return l.join(\"\")}," +
                "parse:function(h){for(var f=h.length,k=[],j=0;j<f;j+=2){k[j>>>3]|=parseInt(h.substr(j,2),16)<<24-4*(j%8)}return u.create(k,f/2)}},d=t.Latin1={stringify:function(h){for(var f=h.words,h=h.sigBytes,k=[],j=0;j<h;j++){k.push(String.fromCharCode(f[j>>>2]>>>24-8*(j%4)&255))}return k.join(\"\")}," +
                "parse:function(h){for(var f=h.length,k=[],j=0;j<f;j++){k[j>>>2]|=(h.charCodeAt(j)&255)<<24-8*(j%4)}return u.create(k,f)}},v=t.Utf8={stringify:function(h){try{return decodeURIComponent(escape(d.stringify(h)))}catch(f){throw Error(\"Malformed UTF-8 data\")}}," +
                "parse:function(b){return d.parse(unescape(encodeURIComponent(b)))}},x=i.BufferedBlockAlgorithm=g.extend({reset:function(){this._data=u.create();this._nDataBytes=0},_append:function(b){\"string\"==typeof b&&(b=v.parse(b));this._data.concat(b);this._nDataBytes+=b.sigBytes}," +
                "_process:function(k){var j=this._data,p=j.words,o=j.sigBytes,n=this.blockSize,l=o/(4*n),l=k?e.ceil(l):e.max((l|0)-this._minBufferSize,0),k=l*n,o=e.min(4*k,o);if(k){for(var m=0;m<k;m+=n){this._doProcessBlock(p,m)}m=p.splice(0,k);j.sigBytes-=o}return u.create(m,o)}," +
                "clone:function(){var b=g.clone.call(this);b._data=this._data.clone();return b},_minBufferSize:0});i.Hasher=x.extend({init:function(){this.reset()},reset:function(){x.reset.call(this);this._doReset()},update:function(b){this._append(b);this._process();return this}," +
                "finalize:function(b){b&&this._append(b);this._doFinalize();return this._hash},clone:function(){var b=x.clone.call(this);b._hash=this._hash.clone();return b},blockSize:16,_createHelper:function(b){return function(f,h){return b.create(h).finalize(f)}}," +
                "_createHmacHelper:function(b){return function(f,h){return w.HMAC.create(b,h).finalize(f)}}});var w=s.algo={};return s}(Math);(function(d){function b(h,l,j,m,o,n,k){h=h+(l&j|~l&m)+o+k;return(h<<n|h>>>32-n)+l}function g(h,l,j,m,o,n,k){h=h+(l&m|j&~m)+o+k;return(h<<n|h>>>32-n)+l}" +
                "function f(h,l,j,m,o,n,k){h=h+(l^j^m)+o+k;return(h<<n|h>>>32-n)+l}function e(h,l,j,m,o,n,k){h=h+(j^(l|~m))+o+k;return(h<<n|h>>>32-n)+l}var s=CryptoJS,i=s.lib,a=i.WordArray,i=i.Hasher,c=s.algo,t=[];" +
                "(function(){for(var h=0;64>h;h++){t[h]=4294967296*d.abs(d.sin(h+1))|0}})();c=c.MD5=i.extend({_doReset:function(){this._hash=a.create([1732584193,4023233417,2562383102,271733878])}," +
                "_doProcessBlock:function(h,m){for(var j=0;16>j;j++){var n=m+j,p=h[n];h[n]=(p<<8|p>>>24)&16711935|(p<<24|p>>>8)&4278255360}for(var n=this._hash.words,p=n[0],o=n[1],l=n[2],k=n[3],j=0;64>j;j+=4){16>j?(p=b(p,o,l,k,h[m+j],7,t[j]),k=b(k,p,o,l,h[m+j+1],12,t[j+1])," +
                "l=b(l,k,p,o,h[m+j+2],17,t[j+2]),o=b(o,l,k,p,h[m+j+3],22,t[j+3])):32>j?(p=g(p,o,l,k,h[m+(j+1)%16],5,t[j]),k=g(k,p,o,l,h[m+(j+6)%16],9,t[j+1]),l=g(l,k,p,o,h[m+(j+11)%16],14,t[j+2]),o=g(o,l,k,p,h[m+j%16],20,t[j+3])):48>j?(p=f(p,o,l,k,h[m+(3*j+5)%16],4,t[j]),k=f(k,p,o,l,h[m+(3*j+8)%16],11,t[j+1])," +
                "l=f(l,k,p,o,h[m+(3*j+11)%16],16,t[j+2]),o=f(o,l,k,p,h[m+(3*j+14)%16],23,t[j+3])):(p=e(p,o,l,k,h[m+3*j%16],6,t[j]),k=e(k,p,o,l,h[m+(3*j+7)%16],10,t[j+1]),l=e(l,k,p,o,h[m+(3*j+14)%16],15,t[j+2]),o=e(o,l,k,p,h[m+(3*j+5)%16],21,t[j+3]))}n[0]=n[0]+p|0;n[1]=n[1]+o|0;n[2]=n[2]+l|0;n[3]=n[3]+k|0}," +
                "_doFinalize:function(){var h=this._data,k=h.words,j=8*this._nDataBytes,l=8*h.sigBytes;k[l>>>5]|=128<<24-l%32;k[(l+64>>>9<<4)+14]=(j<<8|j>>>24)&16711935|(j<<24|j>>>8)&4278255360;h.sigBytes=4*(k.length+1);this._process();h=this._hash.words;for(k=0;4>k;k++){j=h[k],h[k]=(j<<8|j>>>24)&16711935|(j<<24|j>>>8)&4278255360}}});s.MD5=i._createHelper(c);s.HmacMD5=i._createHmacHelper(c)})(Math);";

            ulong room_id = Function.GetRidFromUrl(url);
            if (room_id == 0) return null;

            string rid = string.Format("var rid = {0}; \n", room_id);
            string did = "var did = \"10000000000000000000000000001501\"; \n ";
            string tt = "var tt=parseInt((new Date).getTime()/1e3,10);\n";
            string script = string.Format("(()=>{{ {0} {1} {2} {3} {4} return ub98484234(rid,did,tt); }})(); ",
                CryptoJS, jsondata, rid, did, tt
                );

            ChakraHost chakra = new ChakraHost();
            bool rr = chakra.RunScript(script);
            if (!rr) return null;

            string reqdata = chakra.Result.Text + "&cdn=&rate=-1&ver=Douyu_219012445&iar=1&ive=0";
            byte[] reqDataBin = Encoding.UTF8.GetBytes(reqdata);

            string jsonstr = null;
            using (WebClient client = new WebClient())
            {
                client.Headers.Add(
                    "User-Agent",
                    "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.62 Safari/537.36"
                    );
                client.Headers.Add("Accept", @"application/json, text/plain, */*");
                client.Headers.Add("Accept-Language", @"zh-CN,en-US;q=0.8,zh;q=0.5,en;q=0.3");
                client.Headers.Add("Referer", @"https://www.douyu.com/" + room_id);
                client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                client.Headers.Add("X-Requested-With", "XMLHttpRequest");
                client.Headers.Add("DNT", "1");
                string uri = "https://www.douyu.com/lapi/live/getH5Play/" + room_id;
                try
                {
                    byte[] bytesResult = client.UploadData(
                        uri,
                        "POST",
                        reqDataBin
                        );
                    jsonstr = System.Text.Encoding.UTF8.GetString(bytesResult);
                }
                catch (Exception)
                {
                    return null;
                }
            }
            var dict = JsonHandle.GetDict(jsonstr);
            if (dict["error"] == "0")
                return dict["data.rtmp_url"] + "/" + dict["data.rtmp_live"];
            return null;
        }


    }
}
