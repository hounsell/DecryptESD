module KeySite
{
   export function CopyKey(mev: MouseEvent): void
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

const Keys = document.getElementsByClassName("break");
for (let i = 0; i < Keys.length; i++)
{
   Keys[i].addEventListener("click", KeySite.CopyKey);
};