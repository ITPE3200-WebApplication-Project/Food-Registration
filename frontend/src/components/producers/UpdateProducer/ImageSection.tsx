import { IProducer } from "../../../types";
import { getImageUrl } from "../../../services/file";

interface ImageSectionProps {
  producer: Partial<IProducer & { imageFile: File | null }>;
  onProducerChange: (
    updates: Partial<IProducer & { imageFile: File | null }>
  ) => void;
}

export function ImageSection({
  producer,
  onProducerChange,
}: ImageSectionProps) {
  return (
    <div className="row mb-4">
      <div className="col-md-8">
        <div className="form-group mb-3">
          <label className="form-label">Change Logo Image</label>
          <input
            type="file"
            className="form-control"
            accept="image/*"
            onChange={(e) =>
              onProducerChange({
                imageFile: e.target.files?.[0] || null,
              })
            }
          />
        </div>
      </div>
      <div className="col-md-4">
        {producer.imageUrl && (
          <div className="mb-3">
            <label className="form-label">Current Logo</label>
            <img
              src={getImageUrl(producer.imageUrl)}
              alt="Current Logo"
              className="img-fluid rounded"
            />
          </div>
        )}
      </div>
    </div>
  );
}
