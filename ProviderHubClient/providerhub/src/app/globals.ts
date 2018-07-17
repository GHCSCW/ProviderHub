export const API = { dev: "http://localhost:51660/", prod: "PROD_URL", selectedProvider:"" };//Object.freeze();

export class Globals {
  public selectedProvider: any = ("" as any);
  public getSelectedProvider() { return this.selectedProvider; }
  public setSelectedProvider(p) { this.selectedProvider = p; }
}
