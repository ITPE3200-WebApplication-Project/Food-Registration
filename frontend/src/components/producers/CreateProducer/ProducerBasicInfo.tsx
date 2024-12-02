import { IProducer } from "../../../types";

interface ProducerBasicInfoProps {
  producer: Partial<IProducer & { imageFile: File | null }>;
  onProducerChange: (
    updates: Partial<IProducer & { imageFile: File | null }>
  ) => void;
}

export function ProducerBasicInfo({
  producer,
  onProducerChange,
}: ProducerBasicInfoProps) {
  return (
    <>
      <div className="form-group mb-3">
        <label className="form-label">
          Name<span className="text-danger">*</span>
        </label>
        <input
          className="form-control"
          required
          value={producer.name}
          onChange={(e) => onProducerChange({ name: e.target.value })}
        />
      </div>

      <div className="form-group mb-3">
        <label className="form-label">
          Description<span className="text-danger">*</span>
        </label>
        <textarea
          className="form-control"
          required
          rows={3}
          value={producer.description}
          onChange={(e) => onProducerChange({ description: e.target.value })}
        />
      </div>
    </>
  );
}
