module KeySite
{
    export function copyKey(mev: MouseEvent): void
    {
        const key = this as HTMLDivElement;

        const range = document.createRange();
        range.selectNode(key);
        const selection = window.getSelection();
        selection.removeAllRanges();
        selection.addRange(range);

        document.execCommand("copy");
    }
}

const keys = document.getElementsByClassName("break");
for (let i = 0; i < keys.length; i++)
{
    keys[i].addEventListener("click", KeySite.copyKey);
};