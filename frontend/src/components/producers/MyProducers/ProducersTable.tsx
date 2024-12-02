import { IProducer } from "../../../types";
import { getImageUrl } from "../../../services/file";

interface ProducersTableProps {
  producers: IProducer[];
  onDelete: (id: number, name: string) => void;
}

export function ProducersTable({ producers, onDelete }: ProducersTableProps) {
  return (
    <table className="table table-hover table-bordered mt-2">
      <thead className="table-primary">
        <tr>
          <th>Logo</th>
          <th>Name</th>
          <th>Description</th>
          <th className="text-center">Update</th>
          <th className="text-center">Delete</th>
        </tr>
      </thead>
      <tbody>
        {producers.map((producer) => (
          <tr key={producer.producerId}>
            <td style={{ width: "100px" }}>
              {producer.imageUrl && (
                <img
                  src={getImageUrl(producer.imageUrl)}
                  alt={producer.name}
                  className="img-fluid"
                  style={{ maxHeight: "50px" }}
                />
              )}
            </td>
            <td>{producer.name}</td>
            <td>{producer.description || "N/A"}</td>
            <td className="text-center">
              <a
                href={`/producers/${producer.producerId}/update`}
                className="btn btn-sm btn-warning rounded-pill px-3"
              >
                <i className="bi bi-pencil-square"></i> Update
              </a>
            </td>
            <td className="text-center">
              <button
                className="btn btn-danger btn-sm rounded-pill px-3"
                onClick={() => onDelete(producer.producerId, producer.name)}
              >
                <i className="bi bi-trash"></i> Delete
              </button>
            </td>
          </tr>
        ))}
      </tbody>
    </table>
  );
}
