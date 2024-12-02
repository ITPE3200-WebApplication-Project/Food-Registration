import { IProducer, IProduct } from "../../../types";

interface ProductBasicInfoProps {
  product: Partial<IProduct & { imageFile: File | null }>;
  producers: IProducer[];
  onProductChange: (
    updates: Partial<IProduct & { imageFile: File | null }>
  ) => void;
}

export function ProductBasicInfo({
  product,
  producers,
  onProductChange,
}: ProductBasicInfoProps) {
  return (
    <>
      <div className="form-group mb-3">
        <label className="form-label">
          Name<span className="text-danger">*</span>
        </label>
        <input
          className="form-control"
          value={product.name}
          required
          onChange={(e) => onProductChange({ name: e.target.value })}
        />
      </div>

      <div className="form-group mb-3">
        <label className="form-label">
          Producer<span className="text-danger">*</span>
        </label>
        <select
          className="form-select"
          required
          value={product.producer?.producerId}
          onChange={(e) =>
            onProductChange({
              producer: producers.find(
                (p) => p.producerId === parseInt(e.target.value)
              ),
            })
          }
        >
          {producers.map((p) => (
            <option key={p.producerId} value={p.producerId}>
              {p.name}
            </option>
          ))}
        </select>
      </div>

      <div className="form-group mb-3">
        <label className="form-label">
          Description<span className="text-danger">*</span>
        </label>
        <textarea
          className="form-control"
          required
          value={product.description}
          onChange={(e) => onProductChange({ description: e.target.value })}
        />
      </div>
    </>
  );
}
