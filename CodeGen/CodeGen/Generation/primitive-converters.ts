import _dayjs, { Dayjs as _Dayjs } from 'dayjs';

function _convert_string_TO_string(from: string): string {
    return from;
}

function _convert_string_TO_number(from: string): number {
    return Number(from);
}

function _convert_number_TO_string(from: number): string {
    return from.toString();
}

function _convert_string_TO_bigint(from: string): bigint {
    return BigInt(from);
}

function _convert_bigint_TO_string(from: bigint): string {
    return from.toString();
}

function _convert_boolean_TO_boolean(from: boolean): boolean {
    return from;
}

function _convert_string_TO__Dayjs(from: string): _Dayjs {
    return _dayjs(from);
}

function _convert__Dayjs_TO_string(from: _Dayjs): string {
    return from.toISOString();
}
