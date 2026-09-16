import type { Aggregate } from "./api";

export type Asset = Aggregate & {
  kind: AssetKind;
  file: AssetFile;
  dimensions?: Dimensions | null;
  duration?: string | null;
};

export type AssetFile = {
  name: string;
  extension: string;
  mimeType: string;
  size: number;
};

export type AssetKind = "Image" | "Video";

export type Dimensions = {
  width: number;
  height: number;
};
