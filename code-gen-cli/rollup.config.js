import terser from '@rollup/plugin-terser';
import nodeResolve from '@rollup/plugin-node-resolve';
import commonjs from '@rollup/plugin-commonjs';
import json from '@rollup/plugin-json';

export default {
    input: '.build/index.js',
    output: {
        file: 'out/bundle.js',
        format: 'cjs'
    },
    plugins: [
        json(),
        commonjs(),
        nodeResolve(),
        terser(),
    ],
};
