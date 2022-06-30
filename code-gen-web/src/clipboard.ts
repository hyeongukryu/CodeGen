export const copyToClipboard = (content?: string) => {
    if (content === undefined) {
        return;
    }
    if (navigator && navigator.clipboard && navigator.clipboard.writeText) {
        navigator.clipboard.writeText(content).then(() => { }, () => { });
    }
}