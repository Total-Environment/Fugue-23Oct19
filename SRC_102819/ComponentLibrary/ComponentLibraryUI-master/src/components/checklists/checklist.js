import React, {Component, PropTypes} from 'react';
import {QuantityEvaluationMethod} from './quantity_evaluation_method';
import {SpecificationsSheet} from './specifications_sheet';
import { GeneralTermsAndConditions } from './general_terms_and_conditions';
import { SpecialTermsAndConditions } from './special_terms_and_conditions';
import { QualityChecklist } from './quality_checklist'
import {SFGInspectionChecklist} from "./sfg_inspection_checklist";
import {PackageQualityChecklist} from "./package_quality_checklist";

export class Checklist extends Component {
    componentDidMount() {
        if (this.props.checklistDetails) {
            return;
        }
        this.props.onChecklistFetchRequest(this.props.checklistId);
    }

    render() {
        if (this.props.checklistDetails) {
            if(this.props.checklistDetails.template != undefined) {
                const template = this.props.checklistDetails.template.replace(/ /g, '').replace('&', 'and').toLocaleLowerCase();
                if (template === "inspectionchecklist") {
                    return <QuantityEvaluationMethod details={this.props.checklistDetails}/>;
                }
                else if (template === "specificationsheet") {
                    return <SpecificationsSheet details={this.props.checklistDetails}/>;
                }
                else if (template === "generalpoterms" || template === "generalsoterms" || template === "generalwoterms") {
                    return <GeneralTermsAndConditions details={this.props.checklistDetails}/>;
                }
                else if (template === "specialpoterms" || template === "specialsoterms" || template  === "specialwoterms") {
                    return <SpecialTermsAndConditions details={this.props.checklistDetails}/>;
                }
                else if (template === "qualitychecklist") {
                    return <QualityChecklist details={this.props.checklistDetails}/>;
                }
                else if(template === "sfginspectionchecklist") {
                  return <SFGInspectionChecklist details={this.props.checklistDetails}/>;
                }
                else if(template === "packagequalitychecklist") {
                  return <PackageQualityChecklist details={this.props.checklistDetails}/>;
                }
            }
            return <h3>Unable to render checklist</h3>;
        }
        else if(this.props.error !== '' && this.props.error !== undefined)
        {
            return <h3>{this.props.error}</h3>;
        }
        return <h3>Loading...</h3>;
    }
}

Checklist.propTypes = {
    checklistId: PropTypes.string,
    checklistDetails: PropTypes.shape({
        checkListId: PropTypes.string,
        id: PropTypes.string,
        template: PropTypes.string,
        title: PropTypes.string,
        content: PropTypes.shape({
            entries: PropTypes.arrayOf(PropTypes.shape({
                cells: PropTypes.arrayOf(PropTypes.shape({
                    value: PropTypes.string,
                    key: PropTypes.string,
                })),
                key: PropTypes.number,
            })),
        }),
    }),
    onChecklistFetchRequest: PropTypes.func.isRequired,
    error: PropTypes.string
};
