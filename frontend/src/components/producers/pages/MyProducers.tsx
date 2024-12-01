import { useEffect, useState } from "react";
import type { IProducer } from "../../../types";
import DeleteModal from "../../shared/DeleteModal";
import { producerApi } from "../../../services/api";
import { authService } from "../../../services/auth";
import { Navigate } from "react-router";

export default function MyProducers() {
  const { loggedIn } = authService.getUser();
  const [producers, setProducers] = useState<IProducer[]>([]);

  useEffect(() => {
    producerApi.getMyProducers().then(setProducers);
  }, []);

  const [showModal, setShowModal] = useState(false);
  const [selectedProducer, setSelectedProducer] = useState<{
    id: number;
    name: string;
  } | null>(null);

  if (!loggedIn) {
    return <Navigate to="/login" />;
  }

  const handleShowModal = (id: number, name: string) => {
    setSelectedProducer({ id, name });
    setShowModal(true);
  };

  const handleCloseModal = () => {
    setShowModal(false);
    setSelectedProducer(null);
  };

  const handleConfirmDelete = async (id: number) => {
    //await onDelete(id);
    console.log(id);
    handleCloseModal();
  };

  return (
    <div className="container my-4">
      <h1 className="text-center mb-4">My Producers</h1>
      <div
        className="card shadow-sm"
        style={{
          width: "100%",
          maxWidth: "none",
          height: "100%",
          maxHeight: "none",
        }}
      >
        <div className="card-body px-4 py-2">
          <div
            className="d-flex justify-content-between align-items-center"
            style={{ width: "100%", minHeight: "60px" }}
          >
            <a
              href="/producers/create"
              className="btn btn-success rounded-pill px-3"
            >
              <i className="bi bi-plus-circle"></i> Add New Producer
            </a>
          </div>
          <table className="table table-hover table-bordered mt-2">
            <thead className="table-primary">
              <tr>
                <th>Name</th>
                <th>Description</th>
                <th className="text-center">Update</th>
                <th className="text-center">Delete</th>
              </tr>
            </thead>
            <tbody>
              {producers.map((producer) => (
                <tr key={producer.producerId}>
                  <td>{producer.name}</td>
                  <td>{producer.description}</td>
                  <td className="text-center">
                    <a
                      href={`/producers/update/${producer.producerId}`}
                      className="btn btn-sm btn-warning rounded-pill px-3"
                    >
                      <i className="bi bi-pencil-square"></i> Update
                    </a>
                  </td>
                  <td className="text-center">
                    <button
                      className="btn btn-danger btn-sm rounded-pill px-3"
                      onClick={() =>
                        handleShowModal(producer.producerId, producer.name)
                      }
                    >
                      <i className="bi bi-trash"></i> Delete
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>

      <DeleteModal
        show={showModal}
        itemId={selectedProducer?.producerId || null}
        itemName={selectedProducer?.name || ""}
        onClose={handleCloseModal}
        onConfirm={handleConfirmDelete}
        itemType="producer"
        warningMessage="You can only delete producers that have no associated products."
      />
    </div>
  );
}
