import { IProducer } from "../../../types";
import { getImageUrl } from "../../../services/file";

interface ProducerInfoProps {
  producer: IProducer;
}

export function ProducerInfo({ producer }: ProducerInfoProps) {
  return (
    <div className="producer-info mb-4 p-3 border rounded">
      <div className="row align-items-center">
        <div className="col d-flex align-items-center">
          <img
            src={getImageUrl(producer.imageUrl)}
            alt={producer.name}
            style={{ height: "80px", marginRight: "20px" }}
          />
          <div>
            <h5 className="mb-2">Producer: {producer.name}</h5>
            {producer.description && (
              <span className="text-muted"> ({producer.description})</span>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}
