"use strict";var e=require("fs"),r=require("fs/promises"),t=require("path");const n=process.argv[2].replace(/\/$/,"");n||(console.error("Usage: node update-api.js <server-root>"),process.exit(1));const s="Y"===process.env.CODEGEN_CLI_WATCH;async function i(e,r){try{const t=new URLSearchParams;t.append("swr",e?"true":"false"),t.append("split","true"),t.append("configFilePath",r);const s=await fetch(`${n}/code-gen-api?${t.toString()}`);return await s.json()}catch{}return null}function o(e){if(null===e)throw new Error("Failed to get code");if(e.startsWith("ERROR\nERROR_BEGIN\n")){const r=e.indexOf("\nERROR_END\n"),t=e.substring(18,r);throw new Error(t)}}function c(e){const r=e.split("\n");let t=null,n="";const s=[];function i(){null!==t&&(s.push({fileName:t,content:n}),t=null,n="")}for(const e of r)if(e.startsWith("// __CODEGEN_VERSION_2_FILE_BOUNDARY__ ")){const r=e.split(" ")[2];o=r,i(),t=o}else{if(null===t)throw new Error("Unexpected line outside of file boundary");n+=e+"\n"}var o;return i(),s}async function a(e,n){await r.mkdir(n,{recursive:!0});for(const s of e){const e=t.join(n,s.fileName);await r.writeFile(e,s.content)}}!async function(){const t="src/api/client",n="src/api/server";for(e.existsSync(t)&&e.existsSync(n)||(console.error("src/api/client and src/api/server must exist"),process.exit(1));;){try{const e=await i(!0,"../client.config");o(e);const l=await i(!1,"../server.config");if(o(l),null!==e||null!==l){const s=c(e),i=c(l);await r.rm("src/api/client",{recursive:!0}),await r.rm("src/api/server",{recursive:!0}),await a(s,t),await a(i,n)}if(!s)break}catch(e){console.error(e),s||process.exit(1)}await new Promise((e=>setTimeout(e,1e3)))}}();
