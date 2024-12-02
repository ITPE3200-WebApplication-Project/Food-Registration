import { IProducer } from "../../../types";

interface ImageUploadProps {
  onProducerChange: (
    updates: Partial<IProducer & { imageFile: File | null }>
  ) => void;
}

export function ImageUpload({ onProducerChange }: ImageUploadProps) {
  return (
    <div className="form-group mb-3">
      <label className="form-label">
        Upload Logo Image<span className="text-danger">*</span>
      </label>
      <input
        type="file"
        className="form-control"
        required
        accept="image/*"
        onChange={(e) =>
          onProducerChange({
            imageFile: e.target.files?.[0] || null,
          })
        }
      />
    </div>
  );
}
